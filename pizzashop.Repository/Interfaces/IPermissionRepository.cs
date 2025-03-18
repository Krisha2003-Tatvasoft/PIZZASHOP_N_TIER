using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Repository.Interfaces;

public interface IPermissionRepository
{
    Task<List<RolePermission>> GetPerByRidAsync(int id);

    Task<Permission> GetPerByIdAsync(int id);

    Task UpdateAsync();

     Task<Permission?> GetRolePermissionAsync(string role, string module);
}
