using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IUserRepository
{
    Task<Userslogin?> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);
    Task<Userslogin?>  GetUserLoginByEmailAsync(string email);
    Task UpdateUserLoginAsync(Userslogin userslogin);
}
