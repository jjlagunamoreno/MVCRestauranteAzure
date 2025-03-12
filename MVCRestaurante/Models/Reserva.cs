using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    public class Reserva
    {
        [Key]  // Define la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Generación automática del ID
        public int IdReserva { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; }

        [Required]
        public TimeSpan HoraReserva { get; set; }

        [Required]
        public string Estado { get; set; }
    }
}
