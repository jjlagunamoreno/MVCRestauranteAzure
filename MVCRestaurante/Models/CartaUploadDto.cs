namespace ApiRestaurante.Models
{
    public class CartaUploadDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string TipoPlato { get; set; }
        public IFormFile Imagen { get; set; }
    }

}
