using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVCRestaurante.Filters;
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

        // Vista para los usuarios normales:
        public IActionResult Index()
        {
            var fechaHoy = DateTime.Today;
            List<string> horariosDisponibles = _repo.ObtenerHorariosDisponibles(fechaHoy);

            ViewBag.HorariosDisponibles = horariosDisponibles;
            ViewBag.Reservas = _repo.GetReservas();

            return View();
        }


        [HttpPost]
        public IActionResult ObtenerHorariosDisponibles(DateTime fecha)
        {
            var horarios = _repo.ObtenerHorariosDisponibles(fecha);
            return Json(horarios);
        }

        // Registrar una nueva reserva
        [HttpPost]
        public IActionResult CrearReserva(Reserva reserva)
        {
            if (reserva == null || string.IsNullOrEmpty(reserva.Nombre))
            {
                return Json(new { success = false, message = "Faltan datos para la reserva." });
            }

            // Verificar si el horario está disponible
            if (!_repo.EsHorarioDisponible(reserva.FechaReserva, reserva.HoraReserva))
            {
                return Json(new { success = false, message = "Este horario ya está lleno. Elige otra hora." });
            }

            _repo.CrearReserva(reserva);

            // 🔥 Retornar JSON con el nombre y la hora de la reserva para la alerta
            return Json(new { success = true, nombre = reserva.Nombre, horaReserva = reserva.HoraReserva });
        }


        [HttpPost]
        public IActionResult EliminarReserva(int id)
        {
            _repo.EliminarReserva(id);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ConfirmarReserva(int id)
        {
            _repo.ConfirmarReserva(id);
            return Json(new { success = true });
        }
    }
}
