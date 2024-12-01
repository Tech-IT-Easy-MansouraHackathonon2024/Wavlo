using System.ComponentModel.DataAnnotations;

namespace Authentication.Services;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string password { get; set; }
    [Required]
    [Compare("password", ErrorMessage = "password does not match")]
    public string confirmpassword { get; set; }

}