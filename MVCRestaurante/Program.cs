using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MVCRestaurante.Services;

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
builder.Services.AddTransient<IRepositoryRestaurante, RepositoryRestaurante>();
builder.Services.AddTransient<RepositoryRestaurante>();

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

// AGREGAMOS EL SERVICIO DE NUESTRA API
builder.Services.AddHttpClient<ServiceApiRestaurante>(client =>
{
    client.BaseAddress = new Uri("https://apirestaurante.azurewebsites.net/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// AGREGAR SERVICIO DE AZURE SMS
builder.Services.AddSingleton<ServiceAzureSms>(provider =>
{
    var connectionString = "endpoint=https://servicesmsrestaurantejjlm.europe.communication.azure.com/;accesskey=6BUv5vKbWFx9gjlovZ9zmgatn1XAVp3WL1WdMgb6zJsHCl3MXfT2JQQJ99BDACULyCpfWOEbAAAAAZCS5kWd";
    var fromPhoneNumber = ""; // PON EL NÚMERO QUE TE HAYA DADO AZURE AQUÍ O VACÍO PARA QUE ESTÉ EN MODO PRUEBAS
    return new ServiceAzureSms(connectionString, fromPhoneNumber);
});


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

