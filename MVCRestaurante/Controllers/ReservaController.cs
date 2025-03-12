using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using Restaurante.Repositories;

namespace Restaurante.Controllers
{
    public class ReservasController : Controller
    {
        private readonly RepositoryReserva repository;

        public ReservasController(RepositoryReserva repository)
        {
            this.repository = repository;
        }

        // Mostrar todas las reservas (solo para administradores)
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            List<Reserva> reservas = repository.GetReservas();
            return View(reservas);
        }

        // Formulario para agregar una nueva reserva
        public IActionResult Create()
        {
            return View();
        }

        // Procesar la reserva
        [HttpPost]
        public IActionResult Create(string telefono, string nombre, DateTime fechaReserva, TimeSpan horaReserva)
        {
            string mensaje;
            bool resultado = repository.InsertarReserva(telefono, nombre, fechaReserva, horaReserva, out mensaje);

            if (!resultado)
            {
                ViewBag.Error = mensaje;
                return View();
            }

            return RedirectToAction("Index");
        }

        // Eliminar una reserva (solo admin)
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            repository.EliminarReserva(id);
            return RedirectToAction("Index");
        }
    }
}
