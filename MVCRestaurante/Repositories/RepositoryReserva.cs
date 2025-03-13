using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Models;

namespace MVCRestaurante.Repositories
{
    public class RepositoryReservas
    {
        private readonly RestauranteContext _context;

        public RepositoryReservas(RestauranteContext context)
        {
            _context = context;
        }

        // Obtener todas las reservas para el administrador
        public List<Reserva> GetReservas()
        {
            return _context.Reservas
                .OrderByDescending(r => r.FechaReserva)
                .ToList();
        }

        // Crear una nueva reserva
        public void CrearReserva(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            _context.SaveChanges();
        }

        // Verificar si una fecha y hora ya tiene 4 reservas
        public bool EsHorarioDisponible(DateTime fecha, TimeSpan hora)
        {
            return _context.Reservas.Count(r => r.FechaReserva == fecha && r.HoraReserva == hora) < 4;
        }
    }
}