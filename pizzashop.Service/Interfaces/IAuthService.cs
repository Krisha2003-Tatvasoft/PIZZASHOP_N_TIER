using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IAuthService 
{
    Task<Userslogin?> AuthenticateUser(string email, string password);
    Task<Role?> CheckRole(string role);

    Task<bool> UserExistsAsync(Forget viewmodel);

    Task ResetPassword(ResetPassword model);
}
