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
public class LoansControllerTests
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

    // Flujo completo: crear préstamo (bloquea libro) y luego devolver (libera libro).
    [TestMethod]
    public async Task Create_LocksBook_Then_Return_Unlocks()
    {
        using var db = NewContext();

        // Seed: un libro disponible.
        var book = new Book { Title = "Refactoring", Author = "Martin Fowler" };
        db.Books.Add(book);
        await db.SaveChangesAsync();

        var controller = new LoansController(db);

        // POST /loans: crea el préstamo
        var loanResult = await controller.Create(new LoanCreateDto { BookId = book.Id, BorrowerName = "Ana" });
        var created = loanResult.Result as CreatedAtActionResult;
        Assert.IsNotNull(created);

        // Leemos el payload devuelto (Loan)
        var loan = created!.Value as Loan;
        Assert.IsNotNull(loan);

        // El libro debe haber quedado NO disponible.
        var reloaded = await db.Books.SingleAsync(b => b.Id == book.Id);
        Assert.IsFalse(reloaded.IsAvailable);

        // PUT /loans/return/{id}: devolución
        var ret = await controller.ReturnLoan(loan!.Id);
        Assert.IsInstanceOfType(ret, typeof(NoContentResult));

        // El libro vuelve a estar disponible.
        var freed = await db.Books.SingleAsync(b => b.Id == book.Id);
        Assert.IsTrue(freed.IsAvailable);
    }
}
