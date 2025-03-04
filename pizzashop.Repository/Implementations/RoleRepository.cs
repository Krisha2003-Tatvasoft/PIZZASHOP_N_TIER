using Microsoft.EntityFrameworkCore;
using pizzashop.Repository.Interfaces;
using pizzashop.Entity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.Repository.Implementations;

public class RoleRepository : IRoleRepository
{
     private readonly PizzashopContext _context;

        public RoleRepository(PizzashopContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetRoleByNameAsync(string role)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Rolename == role);
        }

        public async Task<List<SelectListItem>> GetAllRolesAsync() =>
        await _context.Roles.Select
        (c => new SelectListItem 
        { Value = c.Roleid.ToString(), Text = c.Rolename})
        .ToListAsync();

}
