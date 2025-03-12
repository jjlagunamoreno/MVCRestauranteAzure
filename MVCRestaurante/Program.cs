using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Repositories;
using Restaurante.Repositories;

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
builder.Services.AddScoped<RepositoryLogin>();
builder.Services.AddScoped<RepositoryReserva>();

//INYECTAR SEGURIDAD EN LOGIN
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();
builder.Services
    .AddControllersWithViews(options => options.EnableEndpointRouting = false);

// Add services to the container.
builder.Services.AddControllersWithViews();

// AGREGAR CONTROLADORES CON VISTAS
builder.Services
    .AddControllersWithViews
    (options => options.EnableEndpointRouting = false)
    .AddSessionStateTempDataProvider();

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

// DESHABILITAMOS EL RUTEO PARA EL LOGIN
// app.UseRouting();

// HABILITAR AUTENTICACIÓN Y AUTORIZACIÓN
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// CONFIGURAR RUTA POR DEFECTO
app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}"
        );
});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}"
//);

// EJECUTAR LA APLICACIÓN
app.Run();

