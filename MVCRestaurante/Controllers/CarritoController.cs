using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

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

    // Confirmar pedido usando la vista `vw_PedidosActivos`
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
                // 1️⃣ Crear el pedido
                var pedido = new Pedido
                {
                    Telefono = telefono,
                    TipoPedido = tipoPedido,
                    Estado = "Pendiente",
                    FechaPedido = DateTime.Now,
                    HoraPedido = DateTime.Now.TimeOfDay,
                    PrecioTotal = carrito.Sum(p => p.Precio * p.Cantidad)
                };

                _context.Pedidos.Add(pedido);
                _context.SaveChanges(); // Guarda el pedido para obtener el ID generado

                // 2️⃣ Insertar detalles del pedido
                foreach (var item in carrito)
                {
                    var detalle = new DetallePedido
                    {
                        IdPedido = pedido.IdPedido, // Asociar con el pedido recién creado
                        IdPlato = item.IdPlato,
                        Cantidad = item.Cantidad
                    };

                    _context.DetallesPedidos.Add(detalle);
                }

                _context.SaveChanges(); // Guarda los detalles del pedido

                transaction.Commit(); // Confirma la transacción

                // 3️⃣ Vaciar carrito después de confirmar pedido
                carrito.Clear();
                _cache.Set(CarritoSessionKey, carrito);

                return RedirectToAction("PedidoRealizado", new { id = pedido.IdPedido });
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // Revierte la transacción si hay un error
                TempData["Error"] = "Hubo un problema al confirmar el pedido.";
                return RedirectToAction("Index");
            }
        }
    }

    // Vista de pedido realizado (Usando la vista vw_PedidosActivos)
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
