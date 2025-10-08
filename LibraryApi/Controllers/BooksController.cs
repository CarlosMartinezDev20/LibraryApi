using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly LibraryContext _db;
    public BooksController(LibraryContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        => Ok(await _db.Books.AsNoTracking().OrderBy(b => b.Title).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetById(int id)
    {
        var book = await _db.Books.AsNoTracking().SingleOrDefaultAsync(b => b.Id == id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> Create([FromBody] BookCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var book = new Book
        {
            Title = dto.Title.Trim(),
            Author = dto.Author.Trim(),
            Category = dto.Category?.Trim(),
            IsAvailable = true
        };

        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var book = await _db.Books.SingleOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();

        book.Title = dto.Title.Trim();
        book.Author = dto.Author.Trim();
        book.Category = dto.Category?.Trim();
        book.IsAvailable = dto.IsAvailable;
        book.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // 🔴 Soft delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var book = await _db.Books.SingleOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();

        book.IsDeleted = true;
        book.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
