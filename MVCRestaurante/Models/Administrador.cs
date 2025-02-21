using System.ComponentModel.DataAnnotations;

namespace MVCRestaurante.Models
{
    public class Administrador
    {
        [Key]
        public int IdAdmin { get; set; }

        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Contraseña { get; set; }
    }

}
