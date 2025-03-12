using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    [Table("CLIENTES")]
    public class Cliente
    {
        [Key]
        [Column("TELEFONO")]
        public string Telefono { get; set; }

        [Required]
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("DIRECCION")]
        public string? Direccion { get; set; }
        public List<Valoracion> Valoraciones { get; set; } = new List<Valoracion>();
    }
}
