using System.IdentityModel.Tokens.Jwt;
using Authentication.Data;

namespace Authentication.Services;

public interface ITokenService
{
    public Task<string> GenerateJwtToken(ApplicationUser user);

}