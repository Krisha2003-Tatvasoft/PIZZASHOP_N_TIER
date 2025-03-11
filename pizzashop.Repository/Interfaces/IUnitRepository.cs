using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.Repository.Interfaces;

public interface IUnitRepository
{
     Task<List<SelectListItem>> GetAllUnitAsync();
}
