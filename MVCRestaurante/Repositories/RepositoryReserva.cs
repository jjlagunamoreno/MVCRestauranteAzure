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

        public void EliminarReserva(int id)
        {
            var reserva = _context.Reservas.Find(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
                _context.SaveChanges();
            }
        }

        public void ConfirmarReserva(int id)
        {
            var reserva = _context.Reservas.Find(id);
            if (reserva != null)
            {
                reserva.Estado = "Confirmada";
                _context.SaveChanges();
            }
        }

        public bool EsHorarioDisponible(DateTime fecha, TimeSpan hora)
        {
            return _context.Reservas.Count(r => r.FechaReserva == fecha && r.HoraReserva == hora) < 4;
        }

        public List<string> ObtenerHorariosDisponibles(DateTime fecha)
        {
            List<string> horarios = new List<string>();
            for (int hora = 13; hora <= 23; hora++)
            {
                var horaCompleta1 = $"{hora}:00";
                var horaCompleta2 = $"{hora}:30";

                if (EsHorarioDisponible(fecha, TimeSpan.Parse(horaCompleta1)))
                    horarios.Add(horaCompleta1);
                if (EsHorarioDisponible(fecha, TimeSpan.Parse(horaCompleta2)))
                    horarios.Add(horaCompleta2);
            }
            return horarios;
        }

    }
}