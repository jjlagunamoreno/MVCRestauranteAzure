using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using MVCRestaurante.Filters;


public class CarritoController : Controller
{
    // *** CONTEXTO DE BASE DE DATOS PARA ACCEDER A LAS ENTIDADES ***
    private readonly RestauranteContext _context;

    // *** CACHE EN MEMORIA PARA GUARDAR EL CARRITO DE COMPRAS ***
    private readonly IMemoryCache _cache;

    // *** CLAVE DE SESIÓN PARA IDENTIFICAR EL CARRITO EN LA CACHE ***
    private const string CarritoSessionKey = "Carrito";

    // *** CONSTRUCTOR CON INYECCIÓN DE DEPENDENCIAS (DB CONTEXT Y CACHE) ***
    public CarritoController(RestauranteContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    //MÉTODO POST PARA CAMBIAR EL ESTADO DEL PEDIDO EN LA VISTA PedidosActivos
    [HttpPost]
    public IActionResult MarcarPedidoComoCompletado(int id, bool completado)
    {
        var pedido = _context.Pedidos.FirstOrDefault(p => p.IdPedido == id);

        if (pedido != null)
        {
            pedido.Estado = completado ? "ENTREGADO" : "PENDIENTE";
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                id = pedido.IdPedido,
                estado = pedido.Estado,
                fechaPedido = pedido.FechaPedido.ToString("yyyy-MM-dd"), // Formato ISO para comparación
                horaPedido = pedido.HoraPedido.ToString(@"hh\:mm\:ss")  // Se usa para ordenar correctamente
            });
        }

        return Json(new { success = false });
    }

    // ***********************************************************************
    // *** MÉTODO PRIVADO PARA OBTENER EL CARRITO DESDE LA CACHE ***
    // SI NO EXISTE, CREA UN NUEVO CARRITO (LISTA DE DetallePedido)
    // TAMBIÉN ASEGURA QUE CADA ITEM TENGA SU OBJETO PLATO ASOCIADO
    // ***********************************************************************
    private List<DetallePedido> ObtenerCarrito()
    {
        if (!_cache.TryGetValue(CarritoSessionKey, out List<DetallePedido> carrito))
        {
            carrito = new List<DetallePedido>();
        }

        // *** RECORREMOS EL CARRITO PARA VERIFICAR QUE CADA ITEM TENGA EL OBJETO PLATO ***
        foreach (var item in carrito)
        {
            if (item.Plato == null)
            {
                // BUSCAMOS EL PLATO CORRESPONDIENTE SEGÚN SU ID
                item.Plato = _context.Cartas.FirstOrDefault(p => p.IdPlato == item.IdPlato);
            }
        }

        return carrito;
    }

    // ***********************************************************************
    // *** ACCIÓN PARA CONFIRMAR EL PEDIDO (HTTP POST) ***
    // RECIBE LOS DATOS DEL PEDIDO (TELÉFONO, NOMBRE, TIPO, DIRECCIÓN)
    // ***********************************************************************
    [HttpPost]
    public IActionResult ConfirmarPedido(string telefono, string nombre, string tipoPedido, string direccion)
    {
        // *** OBTENEMOS EL CARRITO DESDE LA CACHE ***
        var carrito = ObtenerCarrito();
        if (carrito.Count == 0)
        {
            TempData["Error"] = "El carrito está vacío.";
            return RedirectToAction("Index");
        }

        // *** INICIAMOS UNA TRANSACCIÓN PARA ASEGURAR LA INTEGRIDAD DE LOS DATOS ***
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                // *** CREAMOS UN DATATABLE PARA PASAR LOS DETALLES DEL PEDIDO AL PROCEDIMIENTO ALMACENADO ***
                var detallesPedido = new DataTable();
                detallesPedido.Columns.Add("ID_PLATO", typeof(int));
                detallesPedido.Columns.Add("CANTIDAD", typeof(int));

                // *** RECORREMOS CADA ITEM DEL CARRITO Y LO AÑADIMOS AL DATATABLE ***
                foreach (var item in carrito)
                {
                    detallesPedido.Rows.Add(item.IdPlato, item.Cantidad);
                }

                // *** PARAMETRO DE SALIDA PARA OBTENER EL ID DEL NUEVO PEDIDO ***
                var idPedidoParam = new SqlParameter
                {
                    ParameterName = "@IdPedido",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                // *** EJECUTAMOS EL PROCEDIMIENTO ALMACENADO PARA REGISTRAR EL PEDIDO ***
                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_RealizarPedido @Telefono, @Nombre, @Direccion, @TipoPedido, @DetallesPedido, @IdPedido OUTPUT",
                    new SqlParameter("@Telefono", telefono),
                    new SqlParameter("@Nombre", nombre),
                    // *** SI EL TIPO DE PEDIDO ES DOMICILIO, PASAMOS LA DIRECCIÓN; CASO CONTRARIO, PASAMOS NULL ***
                    new SqlParameter("@Direccion", tipoPedido == "DOMICILIO" ? direccion : (object)DBNull.Value),
                    new SqlParameter("@TipoPedido", tipoPedido),
                    // *** PASAMOS EL DATATABLE CON LOS DETALLES DEL PEDIDO, CON TYPE NAME DEFINIDO ***
                    new SqlParameter("@DetallesPedido", detallesPedido) { TypeName = "dbo.DetallePedidoType" },
                    idPedidoParam
                );

                // *** OBTENEMOS EL ID DEL PEDIDO REGISTRADO DESDE EL PARAMETRO DE SALIDA ***
                int idPedido = (int)idPedidoParam.Value;

                // *** CONFIRMAMOS LA TRANSACCIÓN ***
                transaction.Commit();

                // *** VACIAMOS EL CARRITO DESPUÉS DE CONFIRMAR EL PEDIDO ***
                carrito.Clear();
                _cache.Set(CarritoSessionKey, carrito);

                // *** REDIRIGIMOS A LA VISTA DE PEDIDO REALIZADO, PASANDO EL ID DEL PEDIDO ***
                return RedirectToAction("PedidoRealizado", new { id = idPedido });
            }
            catch (Exception ex)
            {
                // *** EN CASO DE ERROR, REVERTIMOS LA TRANSACCIÓN Y MOSTRAMOS UN MENSAJE DE ERROR ***
                transaction.Rollback();
                Console.WriteLine("ERROR AL REGISTRAR EL PEDIDO: " + ex.Message);
                TempData["Error"] = "Hubo un problema al confirmar el pedido.";
                return RedirectToAction("Index");
            }
        }
    }

    // ***********************************************************************
    // *** ACCIÓN PARA MOSTRAR LA VISTA DEL PEDIDO REALIZADO ***
    // UTILIZA UN PROCEDIMIENTO ALMACENADO O CONSULTA SQL PARA OBTENER LOS DATOS
    // ***********************************************************************
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

    // ***********************************************************************
    // *** ACCIÓN PARA MOSTRAR EL CARRITO ***
    // RECUPERA EL CARRITO DESDE LA CACHE Y SE ASEGURA QUE LOS OBJETOS PLATO ESTÉN ASOCIADOS
    // ***********************************************************************
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

    // ***********************************************************************
    // *** ACCIÓN PARA AGREGAR UN PRODUCTO AL CARRITO ***
    // RECIBE EL ID DEL PLATO, VERIFICA SU EXISTENCIA Y AÑADE/ACTUALIZA LA CANTIDAD
    // ***********************************************************************
    public IActionResult Agregar(int id)
    {
        var carrito = ObtenerCarrito();
        var plato = _context.Cartas.FirstOrDefault(p => p.IdPlato == id);

        if (plato != null)
        {
            var item = carrito.FirstOrDefault(p => p.IdPlato == id);
            if (item != null)
            {
                // *** SI EL ITEM YA EXISTE EN EL CARRITO, INCREMENTAMOS SU CANTIDAD ***
                item.Cantidad++;
            }
            else
            {
                // *** SI NO EXISTE, LO AÑADIMOS CON CANTIDAD 1 Y PRECIO DEL PLATO ***
                carrito.Add(new DetallePedido { IdPlato = id, Cantidad = 1, Precio = plato.Precio });
            }
            // *** ACTUALIZAMOS EL CARRITO EN LA CACHE ***
            _cache.Set(CarritoSessionKey, carrito);
        }

        // *** DEVOLVEMOS EL TOTAL DE PRODUCTOS EN EL CARRITO EN FORMATO JSON ***
        return Json(new { count = carrito.Sum(p => p.Cantidad) });
    }

    // ***********************************************************************
    // *** ACCIÓN PARA MOSTRAR LOS PEDIDOS ACTIVOS ***
    // UTILIZA UNA CONSULTA SQL PARA OBTENER LOS DATOS DE LA VISTA vw_PedidosActivos
    // ***********************************************************************
    [AuthorizeEmpleados]
    public IActionResult PedidosActivos()
    {
        var pedidos = _context.PedidosActivos
                             .FromSqlRaw("SELECT * FROM vw_PedidosActivos")
                             .ToList();

        return View(pedidos);
    }

    // ***********************************************************************
    // *** ACCIÓN PARA ACTUALIZAR LA CANTIDAD DE UN PRODUCTO EN EL CARRITO ***
    // ACTUALIZA LA CANTIDAD Y DEVUELVE EL TOTAL Y SUBTOTAL ACTUALIZADO EN FORMATO JSON
    // ***********************************************************************
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

    // ***********************************************************************
    // *** ACCIÓN GET PARA OBTENER LA CANTIDAD TOTAL DE PRODUCTOS EN EL CARRITO ***
    // DEVUELVE UN OBJETO JSON CON EL TOTAL
    // ***********************************************************************
    [HttpGet]
    public IActionResult ObtenerCantidad()
    {
        var carrito = ObtenerCarrito();
        int cantidadTotal = carrito.Sum(p => p.Cantidad);
        return Json(new { count = cantidadTotal });
    }

    // ***********************************************************************
    // *** ACCIÓN DELETE PARA ELIMINAR UN PRODUCTO DEL CARRITO ***
    // BUSCA EL ITEM POR SU ID Y LO ELIMINA, ACTUALIZANDO LA CACHE
    // ***********************************************************************
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
