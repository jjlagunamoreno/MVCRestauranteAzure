using System.ComponentModel.DataAnnotations;

namespace MVCRestaurante.Models
{
    public class Valoracion
    {
        [Key]
        public int IdValoracion { get; set; }

        [Required]
        public int IdPlato { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public int Valor { get; set; }

        public string Comentario { get; set; }

        public Cliente Cliente { get; set; }
        public Carta Plato { get; set; }
    }

}
