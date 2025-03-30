using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifiersGroupRepository
{
    Task<List<Modifiergroup>> AllModifiersGroup();

    Task<List<SelectListItem>> GetAllMGAsync();

    Task AddNewMG(Modifiergroup modifiergroup);

    Task<Modifiergroup> MGByIdAsync(int id);

     Task UpdateMG(Modifiergroup modifiergroup);

     Task DeleteMG(Modifiergroup modifiergroup);

     Task<bool> MGExistAsync(string MGName);

      Task<bool> MGExistAtEditAsync(string MGName, int id);
}
