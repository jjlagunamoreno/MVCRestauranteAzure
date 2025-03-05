using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    public class Destacado
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("TITULO")]
        public string Titulo { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }

        [Required]
        [Column("IMAGEN")]
        public string Imagen { get; set; } // Nombre de la imagen

        [Required]
        [Column("FECHA_INICIO")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Column("FECHA_FINAL")]
        public DateTime FechaFinal { get; set; }
    }
}
