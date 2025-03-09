using MVCRestaurante.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVCRestaurante.Repositories
{
    public class RepositoryMenu
    {
        private readonly RestauranteContext _context;

        public RepositoryMenu(RestauranteContext context)
        {
            _context = context;
        }

        public List<Menu> GetMenus()
        {
            return _context.Menu.ToList();
        }

        public void InsertarMenu(Menu menu)
        {
            _context.Menu.Add(menu);
            _context.SaveChanges();
        }
    }
}
