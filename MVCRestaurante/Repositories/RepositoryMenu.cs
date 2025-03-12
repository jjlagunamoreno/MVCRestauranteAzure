using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;

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

        public Menu GetMenuById(Guid id)
        {
            return _context.Menu.FirstOrDefault(m => m.IdMenu == id);
        }
        public void InsertarMenu(Menu menu)
        {
            using (var connection = (SqlConnection)_context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_InsertarMenu", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@NOMBRE", SqlDbType.NVarChar, 255) { Value = menu.NombreMenu });
                    command.Parameters.Add(new SqlParameter("@ARCHIVO_PDF", SqlDbType.NVarChar, 255) { Value = menu.PdfRuta });

                    // Parámetro de salida para recibir el ID generado
                    var idOutputParam = new SqlParameter("@ID_GENERADO", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idOutputParam);

                    // Ejecutar el procedimiento almacenado
                    command.ExecuteNonQuery();

                    // Verificar y asignar el ID generado al objeto menú
                    if (idOutputParam.Value != DBNull.Value)
                    {
                        menu.IdMenu = (Guid)idOutputParam.Value;
                    }
                    else
                    {
                        throw new Exception("El procedimiento almacenado no devolvió un ID válido.");
                    }
                }
            }

            // Guardar en Entity Framework Core solo si el ID es válido
            if (menu.IdMenu != Guid.Empty)
            {
                _context.Menu.Add(menu);
                _context.SaveChanges();
            }
        }
        public void EditarMenu(Menu menu)
        {
            using (var connection = (SqlConnection)_context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_EditarMenu", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@ID_MENU", SqlDbType.UniqueIdentifier) { Value = menu.IdMenu });
                    command.Parameters.Add(new SqlParameter("@NOMBRE", SqlDbType.NVarChar, 255) { Value = menu.NombreMenu });
                    command.Parameters.Add(new SqlParameter("@ARCHIVO_PDF", SqlDbType.NVarChar, 255) { Value = menu.PdfRuta });

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("No se pudo actualizar el menú. Verifique que el ID exista.");
                    }
                }
            }
        }
        public void EliminarMenu(Guid id)
        {
            var menu = _context.Menu.FirstOrDefault(m => m.IdMenu == id);
            if (menu != null)
            {
                _context.Menu.Remove(menu);
                _context.SaveChanges();
            }
        }
    }
}
