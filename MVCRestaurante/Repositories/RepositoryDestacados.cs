using MVCRestaurante.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class RepositoryDestacados
{
    private readonly RestauranteContext _context;

    public RepositoryDestacados(RestauranteContext context)
    {
        _context = context;
    }

    // 📌 Obtener todos los destacados (sin filtrar por fecha)
    public List<Destacado> GetDestacados()
    {
        return _context.Destacados
            .OrderBy(d => d.FechaInicio)
            .ToList();
    }

    // 📌 Obtener un destacado por su ID
    public Destacado GetDestacadoById(int id)
    {
        return _context.Destacados.Find(id);
    }

    // 📌 Crear un nuevo destacado con el nombre original de la imagen
    public void CrearDestacado(Destacado destacado)
    {
        if (string.IsNullOrEmpty(destacado.Titulo))
        {
            throw new Exception("El título no puede estar vacío.");
        }

        int nuevoId = (_context.Destacados.Any() ? _context.Destacados.Max(d => d.Id) + 1 : 1);
        destacado.Id = nuevoId;

        _context.Destacados.Add(destacado);
        _context.SaveChanges();
    }

    // 📌 Editar un destacado existente y actualizar la imagen si se sube una nueva
    public void EditarDestacado(Destacado destacado)
    {
        var existente = _context.Destacados.Find(destacado.Id);
        if (existente != null)
        {
            existente.Titulo = destacado.Titulo;
            existente.Descripcion = destacado.Descripcion;
            existente.FechaInicio = destacado.FechaInicio;
            existente.FechaFinal = destacado.FechaFinal;

            // Mantener la imagen original si no se cambia
            if (!string.IsNullOrEmpty(destacado.Imagen))
            {
                existente.Imagen = destacado.Imagen;
            }

            _context.SaveChanges();
        }
    }

    // 📌 Eliminar un destacado por ID
    public void EliminarDestacado(int id)
    {
        var destacado = _context.Destacados.Find(id);
        if (destacado != null)
        {
            // Eliminar imagen del servidor
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/destacados", destacado.Imagen);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            _context.Destacados.Remove(destacado);
            _context.SaveChanges();
        }
    }
}
