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
}
