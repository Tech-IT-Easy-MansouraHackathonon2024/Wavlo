using Authentication.Controllers;
using Authentication.Data;
using EmailService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Authentication.Services;

public class AuthService:IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    public readonly JwtSettings _jwtSettings;

    public AuthService(ITokenService tokenService, UserManager<ApplicationUser> userManager, IEmailSender emailSender
    ,IOptions<JwtSettings> jwtSettings)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _emailSender = emailSender;
    
    }
    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
    {
        var AuthResult = new AuthResultDto();
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            AuthResult.Message= "Invalid Email or Password";
            return AuthResult;
        }
        AuthResult.Token = await _tokenService.GenerateJwtToken(user);
        AuthResult.IsSucceeded= true;
        return AuthResult;
        
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
    {
        var authResult = new AuthResultDto();

        try
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                authResult.Message = "Email already exists";
                return authResult;
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                authResult.Message = "User creation failed";
                authResult.Message = string.Join("\n", result.Errors.Select(e => e.Description));

                return authResult;
            }

            var code = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = code;
            user.CodeExpiration = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);

            var verificationCodeEmail = HtmlTemplate.GetVerificationCodeEmailTemplate(code);
            var emailMessage = new Message(new[] { user.Email }, "Verification Code", verificationCodeEmail);
            _emailSender.SendEmail(emailMessage);

            authResult.IsSucceeded = true;
            authResult.Message = "User registered successfully. Verification code sent to email.";
            return authResult;
        }
        catch (Exception ex)
        {
            authResult.Message = "An error occurred while registering the user.";
            return authResult;
        }
    }

 



    public async Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var resultDto = new ResetPasswordResultDto();

        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            resultDto.Message = "User not found.";
            return resultDto;
        }

        var oldPassword = user.PasswordHash;
        var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.confirmpassword);
        var newPassword = user.PasswordHash;

        if (result.Succeeded)
        {
            resultDto.IsSucceeded = true;
            return resultDto;
        }

        resultDto.Message = "Password reset failed.";
        
        return resultDto;
    }
    




}