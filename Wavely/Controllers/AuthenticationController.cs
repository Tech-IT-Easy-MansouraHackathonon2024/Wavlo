using Authentication.Data;
using Authentication.Services;
using EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Authentication.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly IAuthService _authService;

    public AuthenticationController(UserManager<ApplicationUser> userManager,IEmailSender emailSender
        ,IOptions<JwtSettings> jwtSettings,IAuthService authService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _jwtSettings = jwtSettings;
        _authService = authService;
    }
  
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
       var result =await _authService.LoginAsync(loginDto);
         if (result.IsSucceeded)
         {
              return Ok(result);
         }
         return BadRequest(result);
        
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDto resgisterdto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _authService.RegisterAsync(resgisterdto);
        if (result.IsSucceeded)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("Forgot-Password")]
    public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var code = new Random().Next(100000, 999999).ToString();
        user.VerificationCode =code ;
        user.CodeExpiration = DateTime.UtcNow.AddMinutes(10); 

        await _userManager.UpdateAsync(user);
        var enumrable= new List<string> {request.Email};
        var message = new Message(enumrable, "OTP for password reset", HtmlTemplate.GetVerificationCodeEmailTemplate(code));
        _emailSender.SendEmail(message);    
        return Ok(code);
    }

    [HttpPost("validate-otp")]
    public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (user.VerificationCode == request.Otp && user.CodeExpiration > DateTime.UtcNow)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.VerificationCode = null;
            user.CodeExpiration = null;
            await _userManager.UpdateAsync(user);
            return Ok(token);
        }
        return BadRequest("Invalid or expired OTP.");
        
    }
    

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        var result = await _authService.ResetPasswordAsync(request);

        if (result.IsSucceeded)
        {
            return Ok();
        }

        return BadRequest(new { result.Message });
    }
  
}

public class GenerateOtpRequest
{
    public string Email { get; set; }
}

public class ValidateOtpRequest
{
    public string Email { get; set; }
    public string Otp { get; set; }
}
