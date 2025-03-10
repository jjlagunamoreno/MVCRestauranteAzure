using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Repositories;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURAR BASE DE DATOS CON ENTITY FRAMEWORK
builder.Services.AddDbContext<RestauranteContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlRestaurante")
    )
);

// CONFIGURAR CACHÉ EN MEMORIA
builder.Services.AddMemoryCache();

// INYECTAR REPOSITORIOS
builder.Services.AddScoped<IRepositoryRestaurante, RepositoryRestaurante>();
builder.Services.AddScoped<RepositoryMenu>();

// AGREGAR CONTROLADORES CON VISTAS
builder.Services.AddControllersWithViews();

// CONSTRUIR LA APLICACIÓN
var app = builder.Build();

// CONFIGURAR EL PIPELINE DE MIDDLEWARE
if (!app.Environment.IsDevelopment())
{
    // MANEJO DE ERRORES EN PRODUCCIÓN
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// REDIRECCIONAR A HTTPS
app.UseHttpsRedirection();

// SERVIR ARCHIVOS ESTÁTICOS (CSS, JS, ETC.)
app.UseStaticFiles();

// HABILITAR RUTEO
app.UseRouting();

// HABILITAR AUTORIZACIÓN
app.UseAuthorization();

// CONFIGURAR RUTA POR DEFECTO
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// EJECUTAR LA APLICACIÓN
app.Run();
