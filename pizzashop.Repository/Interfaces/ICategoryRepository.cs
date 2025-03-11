using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ICategoryRepository
{
    Task addAsync(Category category);

    Task<Category> GetCatById(int id);

    List<Category> AllCategory();

    Task UpdateCat(Category category);

   Task<bool> DeleteCat(int id);

   Task<List<SelectListItem>> GetAllCatyAsync();
}
