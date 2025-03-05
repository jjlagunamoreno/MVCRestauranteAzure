using Microsoft.AspNetCore.Mvc;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Authenticate(string Usuario, string Password)
    {
        if (Usuario == "admin" && Password == "1234") // Simulación de autenticación
        {
            return RedirectToAction("Index", "Home"); // Redirige al Home si es correcto
        }

        ViewBag.Error = "Usuario o contraseña incorrectos";
        return View("Index");
    }
}
