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
    private static LibraryContext NewContext()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();
        var options = new DbContextOptionsBuilder<LibraryContext>().UseSqlite(conn).Options;
        var db = new LibraryContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    [TestMethod]
    public async Task Create_And_List_Works()
    {
        using var db = NewContext();
        var controller = new BooksController(db);

        var dto = new BookCreateDto { Title = "DDD", Author = "Eric Evans", Category = "Software" };
        var created = await controller.Create(dto);
        Assert.IsInstanceOfType(created.Result, typeof(CreatedAtActionResult));

        var list = await controller.GetAll();
        var ok = list.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.IsNotNull(ok!.Value);
    }

    [TestMethod]
    public async Task SoftDelete_HidesEntity()
    {
        using var db = NewContext();
        db.Books.Add(new Book { Title = "Clean Code", Author = "Robert C. Martin" });
        await db.SaveChangesAsync();

        var controller = new BooksController(db);
        var del = await controller.SoftDelete(1);
        Assert.IsInstanceOfType(del, typeof(NoContentResult));

        var get = await controller.GetById(1);
        Assert.IsInstanceOfType(get.Result, typeof(NotFoundResult));
    }
}
