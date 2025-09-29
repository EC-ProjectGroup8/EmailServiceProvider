using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class PasswordResetRequest
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Url { get; set; } = null!;
}