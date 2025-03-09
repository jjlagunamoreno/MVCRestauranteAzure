using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCRestaurante.Models
{
    public class Menu
    {
        [Key]
        [Column("ID_MENU")]
        public Guid IdMenu { get; set; } // Se generará en el backend como GUID

        [Required]
        [Column("NOMBRE")]
        public string NombreMenu { get; set; }

        [Required]
        [Column("ARCHIVO_PDF")]
        public string PdfRuta { get; set; } // Guardará solo el nombre del archivo PDF
    }
}
