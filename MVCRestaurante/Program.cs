using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configurar la base de datos con Entity Framework
builder.Services.AddDbContext<RestauranteContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlHospital")));

// Configurar Caché
builder.Services.AddMemoryCache();

// Inyectar repositorios
builder.Services.AddScoped<IRepositoryRestaurante, RepositoryRestaurante>();
builder.Services.AddScoped<RepositoryMenu>();

// Agregar controladores con vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Configurar rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
