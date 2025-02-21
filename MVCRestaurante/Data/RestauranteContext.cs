using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Models;

public class RestauranteContext : DbContext
{
    public RestauranteContext(DbContextOptions<RestauranteContext> options) : base(options) { }

    // Definir las tablas como DbSet
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Administrador> Administradores { get; set; }
    public DbSet<Carta> Cartas { get; set; } 
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<DetallePedido> DetallesPedidos { get; set; }
    public DbSet<Valoracion> Valoraciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carta>().ToTable("CARTA");
        modelBuilder.Entity<Cliente>().ToTable("CLIENTES");
        modelBuilder.Entity<Administrador>().ToTable("ADMINISTRADOR");
        modelBuilder.Entity<Pedido>().ToTable("PEDIDOS");
        modelBuilder.Entity<Reserva>().ToTable("RESERVAS");
        modelBuilder.Entity<DetallePedido>().ToTable("DETALLE_PEDIDOS");
        modelBuilder.Entity<Valoracion>().ToTable("VALORACIONES");

        base.OnModelCreating(modelBuilder);
    }
}
