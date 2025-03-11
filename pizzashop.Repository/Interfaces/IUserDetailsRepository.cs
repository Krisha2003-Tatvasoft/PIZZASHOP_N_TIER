using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IUserDetailsRepository
{
     Task<User?> GetUserByIdAsync(int id);

     Task UpdateAsync(User user);

      Task AddUser(User user);

      Task<bool> Delete(int id);

      Task<bool> PhoneExistsAsync(string phone);
}
