using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    public class DetallePedido
    {
        [Key]
        public int IdDetalle { get; set; }

        [Required]
        [Column("ID_PEDIDO")]
        public int IdPedido { get; set; }

        [Required]
        [Column("ID_PLATO")]
        public int IdPlato { get; set; }

        [Required]
        [Column("CANTIDAD")]
        public int Cantidad { get; set; }

        [Required]
        [Column("PRECIO_UNITARIO")]
        public decimal Precio { get; set; }

        [ForeignKey("IdPedido")]
        public Pedido Pedido { get; set; }

        [ForeignKey("IdPlato")]
        public Carta Plato { get; set; }
    }
}
