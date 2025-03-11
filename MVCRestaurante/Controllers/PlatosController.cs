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
                .ThenInclude(v => v.Cliente)  // Asegurar la carga de cliente
            .FirstOrDefault(p => p.IdPlato == id);

        if (plato == null)
        {
            return NotFound();
        }

        return View(plato);
    }

    // Acción para agregar una valoración
    [HttpPost]
    public IActionResult AgregarValoracion(int idPlato, string telefono, int valor, string comentario)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Telefono == telefono);
        if (cliente == null)
        {
            return Json(new { success = false, message = "Cliente no encontrado." });
        }

        var valoracion = new Valoracion
        {
            IdPlato = idPlato,
            Telefono = telefono,
            Valor = valor,
            Comentario = comentario,
            Cliente = cliente
        };

        _context.Valoraciones.Add(valoracion);
        _context.SaveChanges();

        return Json(new { success = true });
    }
}
