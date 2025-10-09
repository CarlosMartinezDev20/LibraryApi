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

public class CustomWebApplicationFactory : WebApplicationFactory<LibraryApi.Program>
{
    private SqliteConnection? _conn;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseSolutionRelativeContentRoot("LibraryApi")
            .UseSetting(WebHostDefaults.ApplicationKey, typeof(LibraryApi.Program).Assembly.FullName);

        builder.ConfigureServices(services =>
        {
            // Quita el DbContext original (MySQL)
            foreach (var d in services.Where(s => s.ServiceType == typeof(DbContextOptions<LibraryContext>)).ToList())
                services.Remove(d);
            foreach (var d in services.Where(s => s.ServiceType == typeof(LibraryContext)).ToList())
                services.Remove(d);

            // SQLite en memoria
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();
            services.AddDbContext<LibraryContext>(o => o.UseSqlite(_conn!));

            // Crear esquema y seed
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Books.Add(new Book { Title = "Seed Book", Author = "Anon" });
            db.SaveChanges();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _conn?.Dispose();
    }
}
