using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    [Table("ADMINISTRADOR")]
    public class Administrador
    {
        [Key]
        [Column("ID_ADMIN")]
        public int IdAdmin { get; set; }

        [Column("USUARIO")]
        public string Usuario { get; set; }

        [Column("CONTRASENA")]
        public string Contrasena { get; set; }
    }
}
