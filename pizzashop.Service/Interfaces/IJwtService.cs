using System.Security.Claims;

namespace pizzashop.Service.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(string email, int userId, string role);
    ClaimsPrincipal? ValidateToken(string token);
}
