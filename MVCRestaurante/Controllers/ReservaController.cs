using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using MVCRestaurante.Repositories;

namespace MVCRestaurante.Controllers
{
    public class ReservaController : Controller
    {
        private readonly RepositoryReservas _repo;

        public ReservaController(RepositoryReservas repo)
        {
            _repo = repo;
        }

        // Vista para el administrador: Ver todas las reservas
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            var reservas = _repo.GetReservas();
            return View(reservas);
        }

        // Vista para los usuarios normales: Crear reserva
        public IActionResult Index()
        {
            return View();
        }

        // Registrar una nueva reserva
        [HttpPost]
        public IActionResult CrearReserva(Reserva reserva)
        {
            if (!_repo.EsHorarioDisponible(reserva.FechaReserva, reserva.HoraReserva))
            {
                ModelState.AddModelError("", "Este horario ya está lleno. Elige otra hora.");
                return View("Index", reserva);
            }

            _repo.CrearReserva(reserva);
            return RedirectToAction("Index");
        }
    }
}