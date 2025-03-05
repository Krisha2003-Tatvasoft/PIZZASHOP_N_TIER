using pizzashop.Entity.Models;
using VMCategory = pizzashop.Entity.ViewModels.Category;
using pizzashop.Service.Interfaces;
using Org.BouncyCastle.Crypto.Fpe;
using pizzashop.Repository.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace pizzashop.Service.Implementations;

public class CategoryService : ICategoryService
{

  private readonly ICategoryRepository _categoryRepository;

  public CategoryService(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }
  public async Task<bool> AddCategoryAsync(VMCategory model, int loginid)
  {
    Category category = new()
    {
      Categoryname = model.Categoryname,
      Description = model.Description,
      Createdby = loginid,
      Modifiedby = loginid
    };
    await _categoryRepository.addAsync(category);
    return true;
  }

  public List<VMCategory> GetCategoryList()
  {
    var category = _categoryRepository.AllCategory();

     var catList = category.Select(c=> new VMCategory
     {
        Categoryid = c.Categoryid,
        Categoryname= c.Categoryname,
        Description = c.Description,
        Createdby = c.Createdby,
        Modifiedby= c.Modifiedby
     }).ToList();

    return catList;
  }


}
