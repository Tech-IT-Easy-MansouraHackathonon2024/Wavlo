namespace Authentication.Services;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
    Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}