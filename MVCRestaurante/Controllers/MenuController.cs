using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Models;
using MVCRestaurante.Repositories;
using System.Collections.Generic;

public class MenuController : Controller
{
    private readonly RepositoryMenu _repository;

    public MenuController(RepositoryMenu repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        List<Menu> menus = _repository.GetMenus();
        return View(menus);
    }
}
