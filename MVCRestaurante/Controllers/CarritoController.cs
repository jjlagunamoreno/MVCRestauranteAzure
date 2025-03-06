using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;

public class CarritoController : Controller
{
    private readonly RestauranteContext _context;
    private readonly IMemoryCache _cache;
    private const string CarritoSessionKey = "Carrito";

    public CarritoController(RestauranteContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // Obtener carrito desde caché y asociar los platos
    private List<DetallePedido> ObtenerCarrito()
    {
        if (!_cache.TryGetValue(CarritoSessionKey, out List<DetallePedido> carrito))
        {
            carrito = new List<DetallePedido>();
        }

        // Asegurar que cada DetallePedido tenga el objeto Plato asociado
        foreach (var item in carrito)
        {
            if (item.Plato == null)
            {
                item.Plato = _context.Cartas.FirstOrDefault(p => p.IdPlato == item.IdPlato);
            }
        }

        return carrito;
    }
    [HttpPost]
    public IActionResult ConfirmarPedido(string telefono, string nombre, string tipoPedido, string direccion)
    {
        var carrito = ObtenerCarrito();
        if (carrito.Count == 0)
        {
            TempData["Error"] = "El carrito está vacío.";
            return RedirectToAction("Index");
        }

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                // Crear tabla de datos para el procedimiento almacenado
                var detallesPedido = new DataTable();
                detallesPedido.Columns.Add("ID_PLATO", typeof(int));
                detallesPedido.Columns.Add("CANTIDAD", typeof(int));

                foreach (var item in carrito)
                {
                    detallesPedido.Rows.Add(item.IdPlato, item.Cantidad);
                }

                // Parámetro de salida para obtener el ID del nuevo pedido
                var idPedidoParam = new SqlParameter
                {
                    ParameterName = "@IdPedido",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                // Ejecutar el procedimiento almacenado
                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_RealizarPedido @Telefono, @Nombre, @Direccion, @TipoPedido, @DetallesPedido, @IdPedido OUTPUT",
                    new SqlParameter("@Telefono", telefono),
                    new SqlParameter("@Nombre", nombre),
                    new SqlParameter("@Direccion", tipoPedido == "DOMICILIO" ? direccion : (object)DBNull.Value),
                    new SqlParameter("@TipoPedido", tipoPedido),
                    new SqlParameter("@DetallesPedido", detallesPedido) { TypeName = "dbo.DetallePedidoType" },
                    idPedidoParam
                );

                int idPedido = (int)idPedidoParam.Value;

                // Confirmar transacción
                transaction.Commit();

                // Vaciar carrito después de confirmar el pedido
                carrito.Clear();
                _cache.Set(CarritoSessionKey, carrito);

                return RedirectToAction("PedidoRealizado", new { id = idPedido });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Error al registrar el pedido: " + ex.Message);
                TempData["Error"] = "Hubo un problema al confirmar el pedido.";
                return RedirectToAction("Index");
            }
        }
    }

    // Vista de pedido realizado
    public IActionResult PedidoRealizado(int id)
    {
        var pedido = _context.PedidosActivos
                             .FromSqlRaw("SELECT * FROM vw_PedidosActivos WHERE ID_PEDIDO = {0}", id)
                             .AsEnumerable()
                             .FirstOrDefault();

        if (pedido == null)
        {
            return RedirectToAction("Index");
        }

        return View(pedido);
    }


    // Mostrar carrito
    public IActionResult Index()
    {
        var carrito = ObtenerCarrito();

        foreach (var item in carrito)
        {
            if (item.Plato == null)
            {
                item.Plato = _context.Cartas.FirstOrDefault(p => p.IdPlato == item.IdPlato);
            }
        }

        return View(carrito);
    }

    // Agregar producto al carrito
    public IActionResult Agregar(int id)
    {
        var carrito = ObtenerCarrito();
        var plato = _context.Cartas.FirstOrDefault(p => p.IdPlato == id);

        if (plato != null)
        {
            var item = carrito.FirstOrDefault(p => p.IdPlato == id);
            if (item != null)
            {
                item.Cantidad++;
            }
            else
            {
                carrito.Add(new DetallePedido { IdPlato = id, Cantidad = 1, Precio = plato.Precio });
            }
            _cache.Set(CarritoSessionKey, carrito);
        }

        return Json(new { count = carrito.Sum(p => p.Cantidad) });
    }

    // Mostrar pedidos activos (Desde la vista vw_PedidosActivos)
    public IActionResult PedidosActivos()
    {
        var pedidos = _context.PedidosActivos
                             .FromSqlRaw("SELECT * FROM vw_PedidosActivos")
                             .ToList();

        return View(pedidos);
    }

    public IActionResult ActualizarCantidad(int id, int cantidad)
    {
        var carrito = ObtenerCarrito();
        var item = carrito.FirstOrDefault(p => p.IdPlato == id);

        if (item != null)
        {
            item.Cantidad = cantidad;
            _cache.Set(CarritoSessionKey, carrito);
        }

        return Json(new
        {
            total = carrito.Sum(p => p.Precio * p.Cantidad),
            subtotal = item != null ? item.Precio * item.Cantidad : 0
        });
    }

    [HttpDelete]
    public IActionResult Eliminar(int id)
    {
        var carrito = ObtenerCarrito();
        var item = carrito.FirstOrDefault(p => p.IdPlato == id);

        if (item != null)
        {
            carrito.Remove(item);
            _cache.Set(CarritoSessionKey, carrito);
        }

        return Json(new { total = carrito.Sum(p => p.Precio * p.Cantidad) });
    }

}
    