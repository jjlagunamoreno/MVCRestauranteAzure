using System.ComponentModel.DataAnnotations;

namespace MVCRestaurante.Models
{
    public class Cliente
    {
        [Key]
        public string Telefono { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Direccion { get; set; }
    }

}
