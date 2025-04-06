using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IRolePerService
{
    Task<List<Role>> GetAllRoleAsync();
    Task<List<RolePermission>> GetPerTablesAsync(int id);

    Task<bool> UpdatePerAsync(List<RolePermission> updatedPermissions);

    Task<bool> HasPermissionAsync(string role, string module, string permission);

     Task<List<Permission>> GetPermissionById(string role);
}
