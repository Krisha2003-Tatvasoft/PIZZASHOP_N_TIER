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

  private readonly IItemRepository _itemRepository;

  public CategoryService(ICategoryRepository categoryRepository, IItemRepository itemRepository)
  {
    _categoryRepository = categoryRepository;
    _itemRepository = itemRepository;
  }


  public async Task<bool> AddCategoryAsync(VMCategory model, int loginid)
  {
    if (await _categoryRepository.CatExistAsync(model.Categoryname))
    {
      return false;
    }

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


  public async Task<List<VMCategory>> GetCategoryList()
  {
    var category = await _categoryRepository.AllCategory();

    var catList = category.Select(c => new VMCategory
    {
      Categoryid = c.Categoryid,
      Categoryname = c.Categoryname,
      Description = c.Description,
      Createdby = c.Createdby,
      Modifiedby = c.Modifiedby
    }).ToList();

    return catList;
  }

  public async Task<VMCategory> GetCategoryById(int id)
  {
    var GetCatById = await _categoryRepository.GetCatById(id);

    VMCategory category = new VMCategory
    {
      Categoryid = GetCatById.Categoryid,
      Categoryname = GetCatById.Categoryname,
      Description = GetCatById.Description,
      Createdby = GetCatById.Createdby,
      Modifiedby = GetCatById.Modifiedby
    };

    return category;
  }

  public async Task<bool> UpdateCat(VMCategory model)
  {
    var GetCatById = await _categoryRepository.GetCatById(model.Categoryid);

    if (await _categoryRepository.CatNameExistAtEditAsync(model.Categoryname, model.Categoryid))
    {
      return false;
    }
    else
    {
      GetCatById.Categoryname = model.Categoryname;
      GetCatById.Description = model.Description;

      await _categoryRepository.UpdateCat(GetCatById);
      return true;
    }

  }

  public async Task<bool> DeleteCat(int id)
  {
    if (await _categoryRepository.DeleteCat(id))
    {
      await _itemRepository.DeleteByCat(id);
      return true;
    }
    else
    {
      return false;
    }
  }

  public async Task<bool> SaveOrderCategory(List<int> orderIds)
  {
    var category = await _categoryRepository.AllCategoryForOrder();

    if (category == null)
    {
      return false;
    }
    foreach (var item in orderIds)
    {
      var updatesortorder = category.Where(u => u.Categoryid == item).FirstOrDefault();

      if (updatesortorder != null)
      {
        try
        {
          updatesortorder.sortOrder = orderIds.IndexOf(item) + 1;
          await _categoryRepository.UpdateCat(updatesortorder);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
          return false;
        }
      }
    }
    return true;
  }

}
