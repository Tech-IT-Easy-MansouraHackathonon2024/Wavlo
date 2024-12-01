using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services;

public class TokenService:ITokenService
{
    public readonly UserManager<ApplicationUser> _userManager;
    public readonly JwtSettings _jwtSettings;
    public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
    }

    public async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var userclaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
        var claims = new[]
        {
              new Claim(ClaimTypes.NameIdentifier,user.Id),
              new Claim(ClaimTypes.Email,user.Email),   
              
        };
        claims = claims.Concat(userclaims).Concat(roleClaims).ToArray();
        var tokenHandler = new JwtSecurityTokenHandler();
        var symmetricKey = Encoding.UTF8.GetBytes(_jwtSettings.key);
        var siginingCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.issuer,
            Audience = _jwtSettings.audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = siginingCredentials
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString =  new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
   


    }
}