using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cadenaConexion = builder.Configuration.GetConnectionString("EventosDB");
builder.Services.AddDbContext<EventosContext>(options => options.UseSqlServer(cadenaConexion));

builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();

    if (!await context.Categorias.AnyAsync())
    {
        var categorias = new List<Categoria>
        {
            new() { Nombre = "Tecnología" },
            new() { Nombre = "Negocios" },
            new() { Nombre = "Educación" }
        };

        await context.Categorias.AddRangeAsync(categorias);
        await context.SaveChangesAsync();

        var eventos = new List<Evento>
        {
            new()
            {
                Titulo = "Congreso de Desarrollo Web",
                Fecha = DateTime.Today.AddDays(10),
                Lugar = "Auditorio Principal",
                Aforo = 120,
                Precio = 150.00m,
                Modalidad = ModalidadesEvento.Presencial,
                CategoriaId = categorias[0].Id,
                Activo = true
            },
            new()
            {
                Titulo = "Taller de Innovación Empresarial",
                Fecha = DateTime.Today.AddDays(20),
                Lugar = "Sala de Conferencias A",
                Aforo = 40,
                Precio = 80.50m,
                Modalidad = ModalidadesEvento.Presencial,
                CategoriaId = categorias[1].Id,
                Activo = true
            },
            new()
            {
                Titulo = "Seminario de Inteligencia Artificial",
                Fecha = DateTime.Today.AddDays(30),
                Lugar = "Laboratorio 3",
                Aforo = 60,
                Precio = 200.00m,
                Modalidad = ModalidadesEvento.Virtual,
                CategoriaId = categorias[0].Id,
                Activo = true
            },
            new()
            {
                Titulo = "Jornada de Capacitación Docente",
                Fecha = DateTime.Today.AddDays(45),
                Lugar = "Aula Magna",
                Aforo = 90,
                Precio = 0m,
                Modalidad = ModalidadesEvento.Virtual,
                CategoriaId = categorias[2].Id,
                Activo = true
            }
        };

        var asistentes = new List<Asistente>
        {
            new() { Nombres = "Ana Torres Ramírez", Dni = "12345678", Correo = "ana.torres@correo.com", Telefono = "987654321", Activo = true },
            new() { Nombres = "Beto Salazar Vega", Dni = "23456789", Correo = "beto.salazar@correo.com", Telefono = "987654322", Activo = true },
            new() { Nombres = "Carla Mendoza Ríos", Dni = "34567890", Correo = "carla.mendoza@correo.com", Telefono = "987654323", Activo = true },
            new() { Nombres = "Diego Paredes Luna", Dni = "45678901", Correo = "diego.paredes@correo.com", Telefono = "987654324", Activo = true },
            new() { Nombres = "Elena Quispe Flores", Dni = "56789012", Correo = "elena.quispe@correo.com", Telefono = "987654325", Activo = true }
        };

        await context.Eventos.AddRangeAsync(eventos);
        await context.Asistentes.AddRangeAsync(asistentes);
        await context.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
