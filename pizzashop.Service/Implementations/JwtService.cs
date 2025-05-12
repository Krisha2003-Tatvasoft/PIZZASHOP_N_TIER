using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class JwtService : IJwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"];
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
    }

    public string GenerateJwtToken(string email, int userId, string role ,bool rememberMe)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key); // Secret Code (Salt)

        var claims = new List<Claim>
         {

        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Role, role),
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {

            Subject = new ClaimsIdentity(claims),
            Expires =  rememberMe ?  DateTime.UtcNow.AddDays(3) : DateTime.UtcNow.AddHours(1),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateForgetToken(string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);
        var tokenId = Guid.NewGuid().ToString();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim("TokenId", tokenId),
                    new Claim(ClaimTypes.Email, email)
                }),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateCustomerToken(int orderId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);
        var tokenId = Guid.NewGuid().ToString();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim("TokenId", tokenId),
                    new Claim("orderId",orderId.ToString()),
                    new Claim(ClaimTypes.Role, "Customer"),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }



    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
