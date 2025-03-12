using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    [Table("VALORACIONES")]
    public class Valoracion
    {
        [Key]
        [Column("ID_VALORACION")]
        public int IdValoracion { get; set; }

        [Required]
        [Column("ID_PLATO")]
        public int IdPlato { get; set; }

        [Required]
        [Column("TELEFONO")]
        public string Telefono { get; set; }

        [Required]
        [Column("VALORACION")]
        public int Valor { get; set; }

        [Column("COMENTARIO")]
        public string Comentario { get; set; }

        // Relaciones
        [ForeignKey("IdPlato")]
        public virtual Carta Plato { get; set; }

        [ForeignKey("Telefono")]
        public virtual Cliente Cliente { get; set; }
    }
}
