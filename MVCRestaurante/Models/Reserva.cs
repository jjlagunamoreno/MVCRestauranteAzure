using System.ComponentModel.DataAnnotations;

namespace MVCRestaurante.Models
{
    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; }

        [Required]
        public TimeSpan HoraReserva { get; set; }

        [Required]
        public string Estado { get; set; }

        public Cliente Cliente { get; set; }
    }

}
