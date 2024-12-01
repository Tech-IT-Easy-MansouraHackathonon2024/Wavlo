using System.ComponentModel.DataAnnotations;

namespace Authentication.Services;

public class ForgotPasswordDto
{
    [Required] 
    [EmailAddress]
    public string Email { get; set; }
}