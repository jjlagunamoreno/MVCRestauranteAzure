using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    public class Reserva
    {
        [Key]
        [Column("ID_RESERVA")]
        public int IdReserva { get; set; }

        [Required]
        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Required]
        [Column("TELEFONO")]
        public string Telefono { get; set; }

        [Required]
        [Column("FECHA_RESERVA")]
        public DateTime FechaReserva { get; set; }

        [Required]
        [Column("HORA_RESERVA")]
        public TimeSpan HoraReserva { get; set; }

        [Required]
        [Column("CAPACIDAD")]
        public int Personas { get; set; }

        [Required]
        [Column("ESTADO")]
        public string Estado { get; set; } = "PENDIENTE";
    }
}
