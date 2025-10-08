using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class UserRegisterDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; } = null!;
}
