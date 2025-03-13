using MVCRestaurante.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVCRestaurante.Repositories
{
    public class RepositoryCarta
    {
        private readonly RestauranteContext _context;

        public RepositoryCarta(RestauranteContext context)
        {
            _context = context;
        }

        public List<Carta> GetPlatos()
        {
            return _context.Cartas.ToList();
        }
        public void CrearPlato(Carta plato, IFormFile Imagen)
        {
            plato.Activo = "SI"; // Siempre se crean activos

            if (Imagen != null && Imagen.Length > 0)
            {
                string fileName = Path.GetFileName(Imagen.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/platos", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Imagen.CopyTo(stream);
                }

                plato.Imagen = fileName;
            }
            else
            {
                plato.Imagen = "default.jpg"; // Imagen por defecto si no se sube ninguna
            }

            _context.Cartas.Add(plato);
            _context.SaveChanges();
        }

        public void EditarPlato(Carta plato)
        {
            var existente = _context.Cartas.Find(plato.IdPlato);
            if (existente != null)
            {
                existente.Nombre = plato.Nombre;
                existente.Descripcion = plato.Descripcion;
                existente.Precio = plato.Precio;
                existente.TipoPlato = plato.TipoPlato;
                _context.SaveChanges();
            }
        }

        public void CambiarEstadoPlato(int id)
        {
            var plato = _context.Cartas.Find(id);
            if (plato != null)
            {
                plato.Activo = plato.Activo == "SI" ? "NO" : "SI";
                _context.SaveChanges();
            }
        }
    }
}
