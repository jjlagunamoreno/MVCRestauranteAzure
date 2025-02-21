using System.ComponentModel.DataAnnotations;

namespace MVCRestaurante.Models
{
    public class DetallePedido
    {
        [Key]
        public int IdDetalle { get; set; }

        [Required]
        public int IdPedido { get; set; }

        [Required]
        public int IdPlato { get; set; }

        [Required]
        public int Cantidad { get; set; }

        public Pedido Pedido { get; set; }
        public Carta Plato { get; set; }
    }

}
