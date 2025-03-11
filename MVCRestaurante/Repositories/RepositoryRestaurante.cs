using MVCRestaurante.Models;
using Microsoft.EntityFrameworkCore;

public class RepositoryRestaurante : IRepositoryRestaurante
{
    private readonly RestauranteContext _context;

    // CONSTRUCTOR DONDE INYECTAMOS EL CONTEXTO *
    public RepositoryRestaurante(RestauranteContext context)
    {
        _context = context;
    }
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

    // OBTIENE LOS DESTACADOS QUE SIGUEN VIGENTES (FECHAFINAL > DateTime.Now),
    // ORDENADOS POR FECHAINICIO DE MENOR A MAYOR 
    public List<Destacado> GetDestacados()
    {
        return _context.Destacados
            .Where(d => d.FechaFinal > DateTime.Now)
            .OrderBy(d => d.FechaInicio)
            .ToList();
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

    // OBTIENE TODOS LOS PLATOS DISPONIBLES (SIN FILTRO)
    public List<Carta> GetPlatos()
    {
        return _context.Cartas.ToList();
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

    // *** OBTIENE TODAS LAS RESERVAS, INCLUYENDO LA INFORMACIÓN DEL CLIENTE ***
    public List<Reserva> GetReservas()
    {
        return _context.Reservas
            .Include(r => r.Cliente)
            .ToList();
    }

    // *** CREA UNA NUEVA RESERVA EN LA BASE DE DATOS ***
    public void CrearReserva(Reserva reserva)
    {
        _context.Reservas.Add(reserva);
        _context.SaveChanges();
    }
}
