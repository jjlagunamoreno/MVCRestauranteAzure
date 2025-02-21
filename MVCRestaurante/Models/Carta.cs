using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    public class Carta
    {
        [Key]
        [Column("ID_PLATO")] // Mapea con la base de datos
        public int IdPlato { get; set; }

        [Required]
        [Column("NOMBRE")] // Mapea con la base de datos
        public string Nombre { get; set; }

        [Column("DESCRIPCION")] // Mapea con la base de datos
        public string Descripcion { get; set; }

        [Required]
        [Column("PRECIO")] // Mapea con la base de datos
        public decimal Precio { get; set; }

        [Required]
        [Column("TIPO_PLATO")] // 🔹 Mapeo correcto de la columna
        public string TipoPlato { get; set; }

        [Column("IMAGEN")] // Mapea con la base de datos
        public string Imagen { get; set; }
    }
}
