using System.ComponentModel.DataAnnotations;

namespace MVCRestaurante.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string TipoPedido { get; set; }

        [Required]
        public string Estado { get; set; }

        [Required]
        public DateTime FechaPedido { get; set; }

        [Required]
        public TimeSpan HoraPedido { get; set; }

        [Required]
        public decimal PrecioTotal { get; set; }

        public Cliente Cliente { get; set; }
    }

}
