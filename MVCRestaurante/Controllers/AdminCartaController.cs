using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using System.Collections.Generic;

namespace MVCRestaurante.Controllers
{
    public class AdminCartaController : Controller
    {
        private readonly RepositoryRestaurante _repo;

        public AdminCartaController(RepositoryRestaurante repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var platos = _repo.GetPlatos();
            return View(platos);
        }

        [HttpPost]
        public IActionResult Crear(Carta plato, IFormFile Imagen)
        {
            _repo.CrearPlato(plato, Imagen);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Editar(Carta plato, IFormFile Imagen)
        {
            _repo.EditarPlato(plato, Imagen);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CambiarEstado(int id)
        {
            _repo.CambiarEstadoPlato(id);
            return RedirectToAction("Index");
        }
    }
}
