using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifiersGroupRepository
{
    Task<List<Modifiergroup>> AllModifiersGroup();

    Task<List<SelectListItem>> GetAllMGAsync();
}
