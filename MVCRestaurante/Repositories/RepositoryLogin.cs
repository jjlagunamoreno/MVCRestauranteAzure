using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Models;
using System.Threading.Tasks;

namespace MVCRestaurante.Repositories
{
    public class RepositoryLogin
    {
        private readonly RestauranteContext _context;

        public RepositoryLogin(RestauranteContext context)
        {
            _context = context;
        }

        public async Task<Administrador> LogInAdministradorAsync(string usuario, string password)
        {
            return await _context.Administradores
                .FirstOrDefaultAsync(a => a.Usuario == usuario && a.Contrasena == password);
        }
    }
}
