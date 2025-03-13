using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;

public class HomeController : Controller
{
    private readonly IRepositoryRestaurante _repo;
    private readonly RepositoryDestacados _repoDestacados;

    public HomeController(IRepositoryRestaurante repo, RepositoryDestacados repoDestacados)
    {
        _repo = repo;
        _repoDestacados = repoDestacados;
    }

    public IActionResult Index()
    {
        var destacados = _repoDestacados.GetDestacados();
        var categorias = _repo.GetTodasLasCategorias();
        var platos = _repo.GetPlatos();

        var model = new Dictionary<string, List<Carta>>();

        foreach (var categoria in categorias)
        {
            model[categoria] = platos.Where(p => p.TipoPlato == categoria).ToList();
        }

        ViewBag.Destacados = destacados;
        return View(model);
    }

}
