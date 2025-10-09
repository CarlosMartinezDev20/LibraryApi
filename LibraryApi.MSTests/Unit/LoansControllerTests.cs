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

    [TestMethod]
    public async Task Create_LocksBook_Then_Return_Unlocks()
    {
        using var db = NewContext();
        var book = new Book { Title = "Refactoring", Author = "Martin Fowler" };
        db.Books.Add(book);
        await db.SaveChangesAsync();

        var controller = new LoansController(db);
        var loanResult = await controller.Create(new LoanCreateDto { BookId = book.Id, BorrowerName = "Ana" });
        var created = loanResult.Result as CreatedAtActionResult;
        Assert.IsNotNull(created);
        var loan = created!.Value as Loan;
        Assert.IsNotNull(loan);

        var reloaded = await db.Books.SingleAsync(b => b.Id == book.Id);
        Assert.IsFalse(reloaded.IsAvailable);

        var ret = await controller.ReturnLoan(loan!.Id);
        Assert.IsInstanceOfType(ret, typeof(NoContentResult));

        var freed = await db.Books.SingleAsync(b => b.Id == book.Id);
        Assert.IsTrue(freed.IsAvailable);
    }
}
