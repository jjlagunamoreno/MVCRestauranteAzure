using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using MVCRestaurante.Repositories;
using System.Collections.Generic;

namespace MVCRestaurante.Controllers
{
    public class AdminCartaController : Controller
    {
        private readonly RepositoryCarta _repo;

        public AdminCartaController(RepositoryCarta repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var platos = _repo.GetPlatos();
            return View(platos);
        }

        [HttpPost]
        public IActionResult Crear(Carta plato)
        {
            _repo.CrearPlato(plato);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Editar(Carta plato)
        {
            _repo.EditarPlato(plato);
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
