using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models;

public class Book
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required, MaxLength(150)]
    public string Author { get; set; } = null!;

    [MaxLength(100)]
    public string? Category { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public List<Loan> Loans { get; set; } = new();
}
