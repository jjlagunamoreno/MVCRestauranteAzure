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

    // 📌 MÉTODO PARA LISTAR MENÚS
    public IActionResult Index()
    {
        List<Menu> menus = _repository.GetMenus();
        return View(menus);
    }

    // 📌 VISTA PARA CREAR UN MENÚ
    [AuthorizeEmpleados]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [AuthorizeEmpleados]
    public IActionResult Crear(Menu menu, IFormFile PdfFile)
    {
        // ✅ 1. VERIFICAR QUE EL PDF FUE SUBIDO
        if (PdfFile == null || PdfFile.Length == 0)
        {
            ModelState.AddModelError("PdfFile", "El archivo PDF es obligatorio.");
            return View(menu);
        }

        // ✅ 2. GUARDAR EL ARCHIVO PDF EN EL SERVIDOR
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

        menu.PdfRuta = fileName; // Guardar solo el nombre del archivo

        // ✅ 3. INSERTAR EL MENÚ EN LA BASE DE DATOS
        try
        {
            _repository.InsertarMenu(menu);

            // 🚀 VERIFICAR SI SE GENERÓ EL ID
            if (menu.IdMenu == Guid.Empty)
            {
                ModelState.AddModelError("", "Error: No se pudo generar el ID del menú.");
                return View(menu);
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error al insertar el menú: " + ex.Message);
            return View(menu);
        }
    }

    // 📌 MÉTODO PARA EDITAR UN MENÚ
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
        // ✅ 1. SI SE SUBE UN NUEVO ARCHIVO, GUARDARLO
        if (PdfFile != null && PdfFile.Length > 0)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(PdfFile.FileName);
            string filePath = Path.Combine(_rutaArchivos, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                PdfFile.CopyTo(stream);
            }

            menu.PdfRuta = fileName;
        }

        // ✅ 2. ACTUALIZAR EL MENÚ EN LA BASE DE DATOS
        if (ModelState.IsValid)
        {
            _repository.EditarMenu(menu);
            return RedirectToAction("Index");
        }

        return View(menu);
    }

    // 📌 MÉTODO PARA ELIMINAR UN MENÚ
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
