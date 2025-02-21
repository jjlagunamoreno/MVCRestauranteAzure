using MVCRestaurante.Models;
using Microsoft.EntityFrameworkCore;

public class RepositoryRestaurante : IRepositoryRestaurante
{
    private readonly RestauranteContext _context;

    public RepositoryRestaurante(RestauranteContext context)
    {
        _context = context;
    }

    public List<Carta> GetPlatosPorCategoria(string categoria)
    {
        return _context.Cartas.Where(c => c.TipoPlato == categoria).ToList();
    }

    public List<string> GetTodasLasCategorias()
    {
        return _context.Cartas.Select(c => c.TipoPlato).Distinct().ToList();
    }

    public List<Carta> GetPlatos()
    {
        return _context.Cartas.ToList();
    }

    public List<Reserva> GetReservas()
    {
        return _context.Reservas.Include(r => r.Cliente).ToList();
    }

    public List<Pedido> GetPedidos()
    {
        return _context.Pedidos.Include(p => p.Cliente).ToList();
    }

    public void CrearPedido(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
    }

    public void CrearReserva(Reserva reserva)
    {
        _context.Reservas.Add(reserva);
        _context.SaveChanges();
    }
}
