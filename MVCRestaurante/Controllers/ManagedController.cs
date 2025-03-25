using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;

namespace MVCRestaurante.Controllers
{
    public class ManagedController : Controller
    {
        private readonly RepositoryRestaurante repo;

        public ManagedController(RepositoryRestaurante repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            // Buscar administrador en la base de datos
            Administrador administrador = await this.repo.LogInAdministradorAsync(usuario, password);
            if (administrador != null)
            {
                // Crear identidad de usuario
                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, ClaimTypes.Role);

                // Agregar información relevante a los claims
                identity.AddClaim(new Claim(ClaimTypes.Name, administrador.Usuario));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, administrador.IdAdmin.ToString()));

                // Crear el principal con la identidad
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);

                // Autenticar usuario y almacenar en sesión
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal);

                // Redirigir a PedidosActivos tras el login
                return RedirectToAction("PedidosActivos", "Carrito");
            }
            else
            {
                // Si las credenciales no son correctas, mostrar mensaje de error
                ViewData["MENSAJE"] = "Usuario o contraseña incorrectos.";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            // Cerrar sesión del usuario
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
