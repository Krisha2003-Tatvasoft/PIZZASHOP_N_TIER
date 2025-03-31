using VMCategory = pizzashop.Entity.ViewModels.Category;

namespace pizzashop.Service.Interfaces;

public interface ICategoryService 
{
     Task<bool> AddCategoryAsync(VMCategory model,int loginId);
     
     Task<List<VMCategory>> GetCategoryList();

     Task<VMCategory> GetCategoryById(int id);

     Task<bool> UpdateCat(VMCategory model);

     Task<bool> DeleteCat(int id);

     Task<bool> SaveOrderCategory(List<int> orderIds);

}
