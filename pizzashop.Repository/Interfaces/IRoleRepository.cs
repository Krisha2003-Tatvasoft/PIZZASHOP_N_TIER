using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetRoleByNameAsync(string role);

     Task<List<SelectListItem>> GetAllRolesAsync();
}
