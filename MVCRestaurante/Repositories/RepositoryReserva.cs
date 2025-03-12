using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MVCRestaurante.Models;

namespace Restaurante.Repositories
{
    public class RepositoryReserva
    {
        private readonly string connectionString;

        public RepositoryReserva(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Obtener todas las reservas
        public List<Reserva> GetReservas()
        {
            List<Reserva> reservas = new List<Reserva>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT ID_RESERVA, TELEFONO, NOMBRE, FECHA_RESERVA, HORA_RESERVA, ESTADO FROM RESERVAS";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    reservas.Add(new Reserva
                    {
                        IdReserva = reader.GetInt32(0),
                        Telefono = reader.GetString(1),
                        Nombre = reader.GetString(2),
                        FechaReserva = reader.GetDateTime(3),
                        HoraReserva = reader.GetTimeSpan(4),
                        Estado = reader.GetString(5)
                    });
                }
            }
            return reservas;
        }

        // Insertar una reserva
        public bool InsertarReserva(string telefono, string nombre, DateTime fechaReserva, TimeSpan horaReserva, out string mensaje)
        {
            mensaje = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("sp_InsertarReserva", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Telefono", telefono);
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@FechaReserva", fechaReserva);
                command.Parameters.AddWithValue("@HoraReserva", horaReserva);

                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                    return false;
                }
            }
        }

        // Eliminar una reserva por ID
        public void EliminarReserva(int idReserva)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM RESERVAS WHERE ID_RESERVA = @IdReserva";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IdReserva", idReserva);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
