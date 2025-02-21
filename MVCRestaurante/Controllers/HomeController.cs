using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;

public class HomeController : Controller
{
    private readonly IRepositoryRestaurante _repo;

    public HomeController(IRepositoryRestaurante repo)
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        var categorias = _repo.GetTodasLasCategorias(); // Devuelve List<string>
        var platos = _repo.GetPlatos(); // Devuelve List<Carta>

        var model = new Dictionary<string, List<Carta>>();

        foreach (var categoria in categorias)
        {
            model[categoria] = platos.Where(p => p.TipoPlato == categoria).ToList();
        }

        return View(model); // Pasamos el modelo a la vista
    }
}
