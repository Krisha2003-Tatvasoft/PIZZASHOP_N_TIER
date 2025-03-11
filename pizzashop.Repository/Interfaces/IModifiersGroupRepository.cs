using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifiersGroupRepository
{
    List<Modifiergroup> AllModifiersGroup();

    Task<List<SelectListItem>> GetAllMGAsync();
}
