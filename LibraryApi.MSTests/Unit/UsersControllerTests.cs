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
public class UsersControllerTests
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

    //  registrar usuario cuando el email no existe.
    [TestMethod]
    public async Task Register_ReturnsCreated_WhenEmailIsNew()
    {
        using var db = NewContext();
        var controller = new UsersController(db);

        var dto = new UserRegisterDto { FullName = "Test", Email = "test@example.com", Password = "Secreto123" };
        var result = await controller.Register(dto);

        var created = result.Result as CreatedAtActionResult;  // Debe ser 201
        Assert.IsNotNull(created);

        // El body devuelto es un UserResponseDto sin contraseña.
        var payload = created!.Value as UserResponseDto;
        Assert.IsNotNull(payload);
        Assert.AreEqual("test@example.com", payload!.Email);
    }

    // Error esperado: 409 Conflict cuando el email ya existe.
    [TestMethod]
    public async Task Register_ReturnsConflict_WhenEmailExists()
    {
        using var db = NewContext();

        // Seed: usuario con email duplicado.
        db.Users.Add(new User { FullName = "X", Email = "dup@example.com", PasswordHash = "hash" });
        await db.SaveChangesAsync();

        var controller = new UsersController(db);

        var dto = new UserRegisterDto { FullName = "Y", Email = "dup@example.com", Password = "Otro123" };

        var result = await controller.Register(dto);

        // El ActionResult debe ser ConflictObjectResult (409).
        Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));
    }
}
