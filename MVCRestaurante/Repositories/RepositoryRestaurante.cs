using Microsoft.Data.SqlClient;
using MVCRestaurante.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

public class RepositoryRestaurante : IRepositoryRestaurante
{
    private readonly RestauranteContext _context;

    // CONSTRUCTOR DONDE INYECTAMOS EL CONTEXTO
    public RepositoryRestaurante(RestauranteContext context)
    {
        _context = context;
    }

    #region Login
    //MÉTODO PARA REALIZAR EL LOGIN EN NUESTRA APLICACIÓN
    public async Task<Administrador> LogInAdministradorAsync(string usuario, string password)
    {
        return await _context.Administradores
            .FirstOrDefaultAsync(a => a.Usuario == usuario && a.Contrasena == password);
    }
    #endregion

    #region Restaurante
    //MÉTODO PARA CAMBIAR EL ESTADO DEL PEDIDO EN LA VISTA PedidosActivos
    public void CambiarEstadoPedido(int idPedido, string nuevoEstado)
    {
        var pedido = _context.Pedidos.FirstOrDefault(p => p.IdPedido == idPedido);
        if (pedido != null)
        {
            pedido.Estado = nuevoEstado;
            _context.SaveChanges();
        }
    }
    // DEVUELVE TODOS LOS PLATOS DE UNA CATEGORÍA ESPECÍFICA 
    public List<Carta> GetPlatosPorCategoria(string categoria)
    {
        return _context.Cartas
            .Where(c => c.TipoPlato == categoria)
            .ToList();
    }
    // OBTIENE TODAS LAS CATEGORÍAS DISTINTAS DE LA CARTA 
    public List<string> GetTodasLasCategorias()
    {
        return _context.Cartas
            .Select(c => c.TipoPlato)
            .Distinct()
            .ToList();
    }
    // OBTIENE TODOS LOS PEDIDOS, INCLUYENDO EL CLIENTE ASOCIADO
    public List<Pedido> GetPedidos()
    {
        return _context.Pedidos
            .Include(p => p.Cliente)
            .ToList();
    }
    // CREA UN NUEVO PEDIDO EN LA BASE DE DATOS
    public void CrearPedido(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
    }
    //ELIMINAMOS DE LA BBDD LA VALORACIÓN POR SU ID
    public void EliminarValoracion(int id)
    {
        var valoracion = _context.Valoraciones.Find(id);
        if (valoracion != null)
        {
            _context.Valoraciones.Remove(valoracion);
            _context.SaveChanges();
        }
    }
    #endregion

    #region Carta
    // OBTIENE TODOS LOS PLATOS DISPONIBLES (SIN FILTRO) DE LA CARTA
    public List<Carta> GetPlatos()
    {
        return _context.Cartas.ToList();
    }
    //CREAMOS UN PLATO DESDE ADMINISTRADOR GUARDANDO SU IMAGEN EN LOCAL 
    public void CrearPlato(Carta plato, IFormFile Imagen)
    {
        plato.Activo = "SI"; // Siempre se crean activos
        if (Imagen != null && Imagen.Length > 0)
        {
            string fileName = Path.GetFileName(Imagen.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/platos", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Imagen.CopyTo(stream);
            }

            plato.Imagen = fileName;
        }
        else
        {
            plato.Imagen = "default.jpg"; // Imagen por defecto si no se sube ninguna
        }
        _context.Cartas.Add(plato);
        _context.SaveChanges();
    }
    //EDITAMOS CUALQUIERA DE LOS PLATOS DISPONIBLES
    public void EditarPlato(Carta plato, IFormFile Imagen)
    {
        var existente = _context.Cartas.Find(plato.IdPlato);
        if (existente != null)
        {
            existente.Nombre = plato.Nombre;
            existente.Descripcion = plato.Descripcion;
            existente.Precio = plato.Precio;
            existente.TipoPlato = plato.TipoPlato;

            // Si se sube una nueva imagen, actualizarla
            if (Imagen != null && Imagen.Length > 0)
            {
                string fileName = Path.GetFileName(Imagen.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/platos", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Imagen.CopyTo(stream);
                }
                // Eliminar la imagen anterior solo si es diferente
                string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/platos", existente.Imagen);
                if (System.IO.File.Exists(oldImagePath) && existente.Imagen != "default.jpg")
                {
                    System.IO.File.Delete(oldImagePath);
                }
                existente.Imagen = fileName;
            }
            _context.SaveChanges();
        }
    }
    //MÉTODO PARA CAMBIAR EL ESTADO DE ACTIVO A INACTIVO LOS PLATOS PARA QUE NO LO VEAN LOS CLIENTES
    public void CambiarEstadoPlato(int id)
    {
        var plato = _context.Cartas.Find(id);
        if (plato != null)
        {
            plato.Activo = plato.Activo == "SI" ? "NO" : "SI";
            _context.SaveChanges();
        }
    }
    #endregion

    #region Destacados
    // OBTIENE LOS DESTACADOS QUE SIGUEN VIGENTES (FECHAFINAL > DateTime.Now),
    // ORDENADOS POR FECHAINICIO DE MENOR A MAYOR 
    public List<Destacado> GetDestacados()
    {
        return _context.Destacados
            .Where(d => d.FechaFinal > DateTime.Now)
            .OrderBy(d => d.FechaInicio)
            .ToList();
    }
    // OBTENEMOS UN DESTACADO POR SU ID
    public Destacado GetDestacadoById(int id)
    {
        return _context.Destacados.Find(id);
    }
    // CREAMOS UN DESTACADO CON EL NOMBRE ORIGINAL DE LA IMAGEN
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
    // EDITAMOS UN DESTACADO EXISTENTE Y CAMBIAMOS LA IMAGEN EN EL SERVIDOR POR UNA NUEVA
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
    // ELIMINAMOS UN DESTACADO POR SU ID
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
    #endregion

    #region Menu
    // MÉTODO PARA OBTENER TODOS LOS MENUS
    public List<Menu> GetMenus()
    {
        return _context.Menu.ToList();
    }
    // MÉTODO PARA SELECCIONAR UN MENU POR SU ID 
    public Menu GetMenuById(Guid id)
    {
        return _context.Menu.FirstOrDefault(m => m.IdMenu == id);
    }
    // MÉTODO PARA INSERTAR UN NUEVO MENU GUARDANDO SU PDF EN LOCAL
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
    // MÉTODO PARA EDITAR EL MENÚ SELECCIONADO POR EL ADMINISTRADOR
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
    // MÉTODO PARA ELIMINAR EL MENÚ DESEADO POR EL ADMINISTRADOR
    public void EliminarMenu(Guid id)
    {
        var menu = _context.Menu.FirstOrDefault(m => m.IdMenu == id);
        if (menu != null)
        {
            _context.Menu.Remove(menu);
            _context.SaveChanges();
        }
    }
    #endregion

    #region Reserva
    // CREA UNA NUEVA RESERVA EN LA BASE DE DATOS
    public void CrearReserva(Reserva reserva)
    {
        _context.Reservas.Add(reserva);
        _context.SaveChanges();
    }
    // MÉTODO PARA GENERAR LA LISTA CON LAS RESERVAS QUE TENEMOS
    public List<Reserva> GetReservas()
    {
        return _context.Reservas
            .OrderByDescending(r => r.FechaReserva)
            .ToList();
    }
    // MÉTODO PARA ELIMINAR UNA RESERVA REALIZADA
    public void EliminarReserva(int id)
    {
        var reserva = _context.Reservas.Find(id);
        if (reserva != null)
        {
            _context.Reservas.Remove(reserva);
            _context.SaveChanges();
        }
    }
    // MÉTODO PARA CONFIRMAR LA RESERVA DEL COMENSAL
    public void ConfirmarReserva(int id)
    {
        var reserva = _context.Reservas.Find(id);
        if (reserva != null)
        {
            reserva.Estado = "Confirmada";
            _context.SaveChanges();
        }
    }
    // MÉTODO PARA SABER SI EL HORARIO DE LA RESERVA ESTÁ DISPONIBLE
    public bool EsHorarioDisponible(DateTime fecha, TimeSpan hora)
    {
        return _context.Reservas.Count(r => r.FechaReserva == fecha && r.HoraReserva == hora) < 4;
    }
    // MÉTODO PARA OBTENER TODOS LOS HORARIOS DISPONIBLES COMO OPTIONS EN SU SELECT
    public List<string> ObtenerHorariosDisponibles(DateTime fecha)
    {
        List<string> horarios = new List<string>();
        for (int hora = 13; hora <= 23; hora++)
        {
            var horaCompleta1 = $"{hora}:00";
            var horaCompleta2 = $"{hora}:30";

            if (EsHorarioDisponible(fecha, TimeSpan.Parse(horaCompleta1)))
                horarios.Add(horaCompleta1);
            if (EsHorarioDisponible(fecha, TimeSpan.Parse(horaCompleta2)))
                horarios.Add(horaCompleta2);
        }
        return horarios;
    }
    #endregion
}
