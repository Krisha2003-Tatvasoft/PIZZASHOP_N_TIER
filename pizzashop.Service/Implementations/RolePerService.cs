using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class RolePerService : IRolePerService
{
    private readonly IRoleRepository _roleRepository;

    private readonly IPermissionRepository _permissionRepository;

    public RolePerService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<List<Role>> GetAllRoleAsync()
    {
        return await _roleRepository.GetRoleListAsync();
    }

    public async Task<List<RolePermission>> GetPerTablesAsync(int id)
    {
        return await _permissionRepository.GetPerByRidAsync(id);
    }

    public async Task<bool> UpdatePerAsync(List<RolePermission> updatedPermissions)
    {
        foreach (var updatedPermission in updatedPermissions)
        {
            var existingPermission = await _permissionRepository.GetPerByIdAsync(updatedPermission.Permissionid); 
            if (existingPermission != null)
            {
                existingPermission.Canview = updatedPermission.Canview;
                existingPermission.Canaddedit = updatedPermission.Canaddedit;
                existingPermission.Candelete = updatedPermission.Candelete;
            }
        }
        await  _permissionRepository.UpdateAsync();
        return true;

    }

      public async Task<bool> HasPermissionAsync(string role, string module, string permission)
    {
        var rolePermission = await _permissionRepository.GetRolePermissionAsync(role, module);

        if (rolePermission == null)
            return false;

        return permission switch
        {
            "View" => rolePermission.Canview,
            "AddEdit" => rolePermission.Canaddedit,
            "Delete" => rolePermission.Candelete,
            _ => false
        };
    }
}
