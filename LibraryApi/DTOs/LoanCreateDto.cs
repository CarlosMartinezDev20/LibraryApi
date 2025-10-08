using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class LoanCreateDto
{
    [Required]
    public int BookId { get; set; }

    [Required, MaxLength(100)]
    public string BorrowerName { get; set; } = null!;
}
