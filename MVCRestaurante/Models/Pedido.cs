using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    [Table("PEDIDOS")]
    public class Pedido
    {
        [Key]
        [Column("ID_PEDIDO")]
        public int IdPedido { get; set; }

        [Required]
        [Column("TELEFONO")]
        public string Telefono { get; set; }

        [Required]
        [Column("TIPO_PEDIDO")]
        public string TipoPedido { get; set; }

        [Required]
        [Column("ESTADO")]
        public string Estado { get; set; }

        [Required]
        [Column("FECHA_PEDIDO")]
        public DateTime FechaPedido { get; set; }

        [Required]
        [Column("HORA_PEDIDO")]
        public TimeSpan HoraPedido { get; set; }

        [Required]
        [Column("PRECIO_TOTAL")]
        public decimal PrecioTotal { get; set; }

        [ForeignKey("Telefono")] // <- Asegura que la relación se haga con TELEFONO
        public Cliente Cliente { get; set; }
    }
}
