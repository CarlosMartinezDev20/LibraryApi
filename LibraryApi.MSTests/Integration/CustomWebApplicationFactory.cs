using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace LibraryApi.MSTests.Integration;

// Fábrica que personaliza el arranque de la app para pruebas de integración.
public class CustomWebApplicationFactory : WebApplicationFactory<LibraryApi.Program>
{
    // Conexión SQLite en memoria que vivirá mientras dure el factory.
    private SqliteConnection? _conn;

    // Aquí se modifica el hosting de pruebas antes de arrancar el servidor.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            // Le dice al host dónde está el proyecto web (para encontrar appsettings, views, etc.)
            .UseSolutionRelativeContentRoot("LibraryApi")
            // Asegura que el ApplicationKey apunte al ensamblado correcto (Program).
            .UseSetting(WebHostDefaults.ApplicationKey, typeof(LibraryApi.Program).Assembly.FullName);

        builder.ConfigureServices(services =>
        {
            // 1) Quitamos el DbContext original (MySQL) registrado por la API.
            foreach (var d in services.Where(s => s.ServiceType == typeof(DbContextOptions<LibraryContext>)).ToList())
                services.Remove(d);
            foreach (var d in services.Where(s => s.ServiceType == typeof(LibraryContext)).ToList())
                services.Remove(d);

            // 2) Registramos LibraryContext contra SQLite en memoria para que las pruebas
            //    sean rápidas, aisladas y sin tocar tu base real.
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();
            services.AddDbContext<LibraryContext>(o => o.UseSqlite(_conn!));

            // 3) Creamos el esquema y “sembramos” datos iniciales.
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Books.Add(new Book { Title = "Seed Book", Author = "Anon" });
            db.SaveChanges();
        });
    }

    // Cerramos la conexión en memoria cuando la fábrica se desecha.
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _conn?.Dispose();
    }
}
