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

        public void CrearPlato(Carta plato)
        {
            plato.Activo = "SI"; // Siempre se crean activos
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
