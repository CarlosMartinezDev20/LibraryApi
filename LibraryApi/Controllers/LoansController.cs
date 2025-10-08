using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LibraryContext _db;
    public LoansController(LibraryContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetAll()
    {
        var loans = await _db.Loans
            .Include(l => l.Book)
            .AsNoTracking()
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();

        return Ok(loans);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Loan>> GetById(int id)
    {
        var loan = await _db.Loans.Include(l => l.Book)
                                  .AsNoTracking()
                                  .SingleOrDefaultAsync(l => l.Id == id);
        return loan is null ? NotFound() : Ok(loan);
    }

    [HttpPost]
    public async Task<ActionResult<Loan>> Create([FromBody] LoanCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var book = await _db.Books.SingleOrDefaultAsync(b => b.Id == dto.BookId);
        if (book is null) return NotFound($"Book {dto.BookId} not found.");
        if (!book.IsAvailable) return BadRequest("Book is not available.");

        var loan = new Loan
        {
            BookId = dto.BookId,
            BorrowerName = dto.BorrowerName.Trim(),
            StartDate = DateTime.UtcNow,
            Returned = false
        };

        book.IsAvailable = false;
        _db.Loans.Add(loan);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    [HttpPut("return/{id:int}")]
    public async Task<IActionResult> ReturnLoan(int id)
    {
        var loan = await _db.Loans.Include(l => l.Book).SingleOrDefaultAsync(l => l.Id == id);
        if (loan is null) return NotFound();
        if (loan.Returned) return BadRequest("Loan already returned.");

        loan.Returned = true;
        loan.EndDate = DateTime.UtcNow;

        if (loan.Book is not null)
            loan.Book.IsAvailable = true;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // 🔴 Soft delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var loan = await _db.Loans.SingleOrDefaultAsync(l => l.Id == id);
        if (loan is null) return NotFound();

        loan.IsDeleted = true;
        loan.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
