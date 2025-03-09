using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Models;

public class RestauranteContext : DbContext
{
    public RestauranteContext(DbContextOptions<RestauranteContext> options) : base(options) { }

    //Definir las vistas
    public DbSet<VwPedidoActivo> PedidosActivos { get; set; }


    // Definir las tablas como DbSet
    public DbSet<Destacado> Destacados { get; set; }
    public DbSet<Menu> Menu { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Administrador> Administradores { get; set; }
    public DbSet<Carta> Cartas { get; set; } 
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<DetallePedido> DetallesPedidos { get; set; }
    public DbSet<Valoracion> Valoraciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Menu>()
                .HasKey(m => m.IdMenu);

        modelBuilder.Entity<VwPedidoActivo>().HasNoKey().ToView("vw_PedidosActivos");

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
