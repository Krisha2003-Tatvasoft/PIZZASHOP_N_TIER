using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetRoleByNameAsync(string role);
}
