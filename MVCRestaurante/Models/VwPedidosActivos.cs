using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    [Table("vw_PedidosActivos")]
    public class VwPedidoActivo
    {
        [Key]
        [Column("ID_PEDIDO")]
        public int IdPedido { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("TIPO_PEDIDO")]
        public string TipoPedido { get; set; }

        [Column("ESTADO")]
        public string Estado { get; set; }

        [Column("FECHA_PEDIDO")]
        public DateTime FechaPedido { get; set; }

        [Column("HORA_PEDIDO")]
        public TimeSpan HoraPedido { get; set; }

        [Column("PRECIO_TOTAL")]
        public decimal PrecioTotal { get; set; }
    }
}
