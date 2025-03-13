using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    public class Carta
    {
        [Key]
        [Column("ID_PLATO")]
        public int IdPlato { get; set; }

        [Required]
        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }

        [Required]
        [Column("PRECIO")]
        public decimal Precio { get; set; }

        [Required]
        [Column("TIPO_PLATO")]
        public string TipoPlato { get; set; }

        [Column("IMAGEN")]
        public string Imagen { get; set; }

        [Required]
        [Column("ACTIVO")]
        public string Activo { get; set; } = "SI";

        public List<Valoracion> Valoraciones { get; set; } = new List<Valoracion>();
    }
}
