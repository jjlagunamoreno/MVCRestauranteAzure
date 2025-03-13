using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using System;
using System.IO;

public class DestacadosController : Controller
{
    private readonly RepositoryDestacados _repo;

    public DestacadosController(RepositoryDestacados repo)
    {
        _repo = repo;
    }

    // 📌 Mostrar todos los destacados
    public IActionResult Index()
    {
        var destacados = _repo.GetDestacados();
        return View(destacados);
    }

    // 📌 Crear un nuevo destacado con el nombre original del archivo
    [HttpPost]
    public IActionResult Crear(IFormFile Imagen, string Titulo, string Descripcion, string FechaInicio, string FechaFinal)
    {
        if (string.IsNullOrEmpty(Titulo) || Imagen == null || Imagen.Length == 0 || string.IsNullOrEmpty(FechaInicio) || string.IsNullOrEmpty(FechaFinal))
        {
            return BadRequest("Todos los campos son obligatorios, incluyendo la imagen y las fechas.");
        }

        if (!DateTime.TryParse(FechaInicio, out DateTime fechaInicioParsed) || !DateTime.TryParse(FechaFinal, out DateTime fechaFinalParsed))
        {
            return BadRequest("Formato de fecha inválido.");
        }

        // Guardar la imagen
        string fileName = Path.GetFileName(Imagen.FileName);
        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/destacados", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            Imagen.CopyTo(stream);
        }

        // Crear objeto Destacado
        Destacado nuevoDestacado = new Destacado
        {
            Titulo = Titulo.Trim(),
            Descripcion = !string.IsNullOrEmpty(Descripcion) ? Descripcion.Trim() : "Sin descripción",
            Imagen = fileName,
            FechaInicio = fechaInicioParsed,
            FechaFinal = fechaFinalParsed
        };

        _repo.CrearDestacado(nuevoDestacado);
        return RedirectToAction("Index");
    }

    // 📌 Editar un destacado existente y actualizar la imagen si hay una nueva
    [HttpPost]
    public IActionResult Editar(int Id, string Titulo, string Descripcion, string FechaInicio, string FechaFinal, IFormFile Imagen)
    {
        var destacado = _repo.GetDestacadoById(Id);
        if (destacado == null)
            return NotFound();

        if (!DateTime.TryParse(FechaInicio, out DateTime fechaInicioParsed) || !DateTime.TryParse(FechaFinal, out DateTime fechaFinalParsed))
        {
            return BadRequest("Formato de fecha inválido.");
        }

        destacado.Titulo = Titulo;
        destacado.Descripcion = Descripcion;
        destacado.FechaInicio = fechaInicioParsed;
        destacado.FechaFinal = fechaFinalParsed;

        // Si hay una nueva imagen, reemplazamos la anterior
        if (Imagen != null && Imagen.Length > 0)
        {
            string fileName = Path.GetFileName(Imagen.FileName);
            string newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/destacados", fileName);

            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                Imagen.CopyTo(stream);
            }

            // Eliminar la imagen anterior solo si es diferente
            string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/destacados", destacado.Imagen);
            if (System.IO.File.Exists(oldPath) && destacado.Imagen != fileName)
            {
                System.IO.File.Delete(oldPath);
            }

            destacado.Imagen = fileName;
        }

        _repo.EditarDestacado(destacado);

        // 🔥 Enviar respuesta JSON en lugar de redirigir
        return Json(new { success = true, message = "Destacado editado correctamente." });
    }

    // 📌 Eliminar un destacado
    [HttpPost]
    public IActionResult Eliminar(int id)
    {
        _repo.EliminarDestacado(id);
        return RedirectToAction("Index");
    }
}
