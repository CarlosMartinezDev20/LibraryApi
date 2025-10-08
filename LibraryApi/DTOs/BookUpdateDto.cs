using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class BookUpdateDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required, MaxLength(150)]
    public string Author { get; set; } = null!;

    [MaxLength(100)]
    public string? Category { get; set; }

    public bool IsAvailable { get; set; }
}
