using pizzashop.Repository.Models;

namespace pizzashop.Repository.Interfaces;

public interface IUserRepository
{
    Task<Userslogin?> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);

}
