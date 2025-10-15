using System.Threading.Tasks;
using LibraryApi.Controllers;
using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryApi.MSTests.Unit;

[TestClass]
public class BooksControllerTests
{
    // Crea un LibraryContext con SQLite en memoria y esquema creado.
    private static LibraryContext NewContext()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();
        var options = new DbContextOptionsBuilder<LibraryContext>().UseSqlite(conn).Options;
        var db = new LibraryContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    //crear un libro y luego obtener el listado.
    [TestMethod]
    public async Task Create_And_List_Works()
    {
        using var db = NewContext();
        var controller = new BooksController(db);

        // POST: crea un libro
        var dto = new BookCreateDto { Title = "DDD", Author = "Eric Evans", Category = "Software" };
        var created = await controller.Create(dto);
        // Verificamos que el resultado sea CreatedAtAction (201).
        Assert.IsInstanceOfType(created.Result, typeof(CreatedAtActionResult));

        // GET: listado
        var list = await controller.GetAll();
        var ok = list.Result as OkObjectResult;
        Assert.IsNotNull(ok);         // Debe ser 200
        Assert.IsNotNull(ok!.Value);  // Y la lista no debe ser null
    }

    // Soft delete: tras eliminar lógicamente, el recurso no debe aparecer ni poder consultarse.
    [TestMethod]
    public async Task SoftDelete_HidesEntity()
    {
        using var db = NewContext();
        // Seed: insertamos un libro.
        db.Books.Add(new Book { Title = "Clean Code", Author = "Robert C. Martin" });
        await db.SaveChangesAsync();

        var controller = new BooksController(db);

        // DELETE: soft delete → 204 NoContent.
        var del = await controller.SoftDelete(1);
        Assert.IsInstanceOfType(del, typeof(NoContentResult));

        // GET by id ahora debe devolver 404 NotFound.
        var get = await controller.GetById(1);
        Assert.IsInstanceOfType(get.Result, typeof(NotFoundResult));
    }
}
