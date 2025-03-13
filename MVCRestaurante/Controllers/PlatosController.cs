using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class PlatosController : Controller
{
    private readonly RestauranteContext _context;

    public PlatosController(RestauranteContext context)
    {
        _context = context;
    }

    // Acción para ver detalles de un plato
    public IActionResult Detalle(int id)
    {
        var plato = _context.Cartas
            .Include(p => p.Valoraciones)
                .ThenInclude(v => v.Cliente)  // Carga explícita de Cliente
            .FirstOrDefault(p => p.IdPlato == id);

        if (plato == null)
        {
            return NotFound();
        }

        return View(plato);
    }

    // Acción para agregar una valoración
    [HttpPost]
    public IActionResult AgregarValoracion([FromBody] Valoracion valoracionInput)
    {
        if (valoracionInput == null || string.IsNullOrEmpty(valoracionInput.Telefono))
        {
            return Json(new { success = false, message = "Datos inválidos." });
        }

        var cliente = _context.Clientes
            .FirstOrDefault(c => c.Telefono == valoracionInput.Telefono);

        if (cliente == null)
        {
            return Json(new { success = false, message = "Cliente no encontrado." });
        }

        // Si Direccion es NULL, se permite el registro
        if (cliente.Direccion == null)
        {
            cliente.Direccion = "No especificada";  // Valor predeterminado opcional
        }


        var platoExiste = _context.Cartas.Any(p => p.IdPlato == valoracionInput.IdPlato);
        if (!platoExiste)
        {
            return Json(new { success = false, message = "El plato no existe." });
        }

        var nuevaValoracion = new Valoracion
        {
            IdPlato = valoracionInput.IdPlato,
            Telefono = valoracionInput.Telefono,
            Valor = valoracionInput.Valor,
            Comentario = string.IsNullOrWhiteSpace(valoracionInput.Comentario) ? "Sin comentario" : valoracionInput.Comentario,
            Cliente = cliente
        };

        _context.Valoraciones.Add(nuevaValoracion);
        _context.SaveChanges();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult EliminarValoracion(int id)
    {
        var valoracion = _context.Valoraciones.Find(id);
        if (valoracion != null)
        {
            _context.Valoraciones.Remove(valoracion);
            _context.SaveChanges();
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Valoración no encontrada." });
    }

}
