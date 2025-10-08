using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class UserUpdateDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = null!;

    // opcional cambiar email (si lo permites)
    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    // opcional cambiar password (si viene, se re-hashea)
    [MinLength(6), MaxLength(100)]
    public string? NewPassword { get; set; }
}
