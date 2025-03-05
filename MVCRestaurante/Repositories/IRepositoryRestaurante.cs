using MVCRestaurante.Models;

public interface IRepositoryRestaurante
{
    List<Destacado> GetDestacados();
    List<Carta> GetPlatosPorCategoria(string categoria);
    List<Carta> GetPlatos();
    List<string> GetTodasLasCategorias();
    List<Reserva> GetReservas();
    List<Pedido> GetPedidos();
    void CrearPedido(Pedido pedido);
    void CrearReserva(Reserva reserva);
}
