using System.Security.Claims;

namespace pizzashop.Service.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(string email, int userId, string role , bool RememberMe);

    string GenerateForgetToken(string email);

    string GenerateCustomerToken(int orderId ,string encodeId);

    ClaimsPrincipal? ValidateToken(string token);
   
}
