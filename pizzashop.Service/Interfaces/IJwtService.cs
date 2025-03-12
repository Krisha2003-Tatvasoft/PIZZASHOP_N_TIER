using System.Security.Claims;

namespace pizzashop.Service.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(string email, int userId, string role);

    string GenerateForgetToken(string email);

    ClaimsPrincipal? ValidateToken(string token);
}
