using Microsoft.AspNetCore.Mvc;
using MVCRestaurante.Filters;
using MVCRestaurante.Models;
using MVCRestaurante.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

public class MenuController : Controller
{
    private readonly RepositoryMenu _repository;
    private readonly string _rutaArchivos = "wwwroot/images/menu/";
    private readonly IWebHostEnvironment _webHostEnvironment;

    public MenuController(RepositoryMenu repository, IWebHostEnvironment webHostEnvironment)
    {
        _repository = repository;
        _webHostEnvironment = webHostEnvironment;
    }

    //MÉTODO PARA LISTAR MENÚS
    public IActionResult Index()
    {
        List<Menu> menus = _repository.GetMenus();
        return View(menus);
    }

    //VISTA PARA CREAR UN MENÚ
    [AuthorizeEmpleados]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [AuthorizeEmpleados]
    public IActionResult Crear(Menu menu, IFormFile PdfFile)
    {
        if (PdfFile == null || PdfFile.Length == 0)
        {
            ModelState.AddModelError("PdfFile", "El archivo PDF es obligatorio.");
            return View(menu);
        }

        string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/menu");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = Path.GetFileName(PdfFile.FileName);
        string filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            PdfFile.CopyTo(stream);
        }

        menu.PdfRuta = fileName;

        try
        {
            _repository.InsertarMenu(menu);
            return RedirectToAction("Index", "Menu");  // 🔥 Redirige correctamente
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error al insertar el menú: " + ex.Message);
            return View(menu);
        }
    }

    //VISTA PARA EDITAR UN MENÚ
    [AuthorizeEmpleados]
    public IActionResult Editar(Guid id)
    {
        var menu = _repository.GetMenuById(id);
        if (menu == null)
        {
            return NotFound();
        }
        return View(menu);
    }
    [HttpPost]
    [AuthorizeEmpleados]
    public IActionResult Editar(Menu menu, IFormFile PdfFile)
    {
        var menuExistente = _repository.GetMenuById(menu.IdMenu);
        if (menuExistente == null)
        {
            return NotFound();
        }

        if (PdfFile != null && PdfFile.Length > 0)
        {
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/menu");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = Path.GetFileName(PdfFile.FileName);
            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                PdfFile.CopyTo(stream);
            }

            menu.PdfRuta = fileName;
        }
        else
        {
            menu.PdfRuta = menuExistente.PdfRuta;
        }

        try
        {
            _repository.EditarMenu(menu);
            return RedirectToAction("Index", "Menu");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error al actualizar el menú: " + ex.Message);
            return View(menu);
        }
    }

    //MÉTODO PARA ELIMINAR UN MENÚ
    [AuthorizeEmpleados]
    [HttpPost]
    public IActionResult Eliminar(Guid id)
    {
        try
        {
            _repository.EliminarMenu(id);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
