using pizzashop.Entity.Models;

namespace pizzashop.Service.Interfaces;

public interface IAuthService 
{
    Task<Userslogin?> AuthenticateUser(string email, string password);
    Task<Role?> CheckRole(string role);
}
