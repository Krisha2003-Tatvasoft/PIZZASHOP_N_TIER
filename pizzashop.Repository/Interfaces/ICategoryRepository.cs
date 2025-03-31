using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ICategoryRepository
{
    Task addAsync(Category category);

    Task<Category> GetCatById(int id);

    Task<List<Category>> AllCategory();

    Task UpdateCat(Category category);

   Task<bool> DeleteCat(int id);

   Task<List<SelectListItem>> GetAllCatyAsync();

    Task<bool> CatExistAsync(string Categoryname);

    Task<bool> CatNameExistAtEditAsync(string Categoryname, int id);

    Task<List<Category>> AllCategoryForOrder();
}
