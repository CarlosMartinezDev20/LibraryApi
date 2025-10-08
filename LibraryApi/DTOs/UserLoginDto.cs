using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class UserLoginDto
{
    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; } = null!;
}
