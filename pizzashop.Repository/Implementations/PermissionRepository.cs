using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly PizzashopContext _context;

    public PermissionRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<RolePermission>> GetPerByRidAsync(int id)
    {
        var roleList = await _context.Permissions
           .Include(u => u.Module)
           .Include(u => u.Role)
           .Where(u => u.Roleid == id)
           .OrderByDescending(u => u.Module.Moduleid)
           .Select(u => new RolePermission
           {
               Permissionid = u.Permissionid,
               Rolename = u.Role.Rolename,
               Permissionname = u.Module.Modulename,
               Canview = u.Canview,
               Canaddedit = u.Canaddedit,
               Candelete = u.Candelete,
           }
           ).ToListAsync();
           
          return roleList; 
    }

    public async Task<Permission> GetPerByIdAsync(int id)
    {
        return await _context.Permissions.FindAsync(id);
    }

    public async Task UpdateAsync()
    {
        await _context.SaveChangesAsync();
    }

}
