using VMCategory = pizzashop.Entity.ViewModels.Category;

namespace pizzashop.Service.Interfaces;

public interface ICategoryService 
{
     Task<bool> AddCategoryAsync(VMCategory model,int loginId);
     
     List<VMCategory> GetCategoryList();

}
