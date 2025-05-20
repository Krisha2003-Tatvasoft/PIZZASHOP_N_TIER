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

  private readonly IOrderRepository _orderRepository;

  public CategoryService(ICategoryRepository categoryRepository, IItemRepository itemRepository
  , IOrderRepository orderRepository)
  {
    _categoryRepository = categoryRepository;
    _itemRepository = itemRepository;
    _orderRepository = orderRepository;
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

  public async Task<List<VMCategory>> GetMenuCategoryList()
  {
    var category = await _categoryRepository.AllCategory();

    var catList = category.Select(c => new VMCategory
    {
      Categoryid = c.Categoryid,
      Categoryname = c.Categoryname,
    }).ToList();
    catList.Insert(0, new VMCategory
    {
      Categoryid = 0,
      Categoryname = "All"
    });
    catList.Insert(0, new VMCategory
    {
      Categoryid = -1,
      Categoryname = "Favorite Items"
    });

    return catList;
  }

  public async Task<List<VMCategory>> GetKOTCategoryList(string status)
  {
    // var categories = await _categoryRepository.AllCategory();
    // var orders = await _orderRepository.InprogressOrders();

    // var catList = categories?.Select(c => new VMCategory
    // {
    //   Categoryid = c.Categoryid,
    //   Categoryname = c.Categoryname,
    //   OrderCount = 0
    // }).ToList() ?? new List<VMCategory>();

    // catList.Insert(0, new VMCategory
    // {
    //   Categoryid = 0,
    //   Categoryname = "All",
    //   OrderCount = 0
    // });

    // // categoryId → HashSet of orderIds
    // var categoryOrderIds = new Dictionary<int, HashSet<int>>();

    // foreach (var order in orders)
    // {
    //   var orderDetails = await _orderRepository.OrderDetailsByIdAsync(order.Orderid);

    //   List<Ordereditem> itemsByFilter;

    //   if (status == "Inprogress")
    //   {
    //     itemsByFilter = orderDetails.Ordereditems
    //         .Where(i => (i.Quantity - i.ReadyQuantity) > 0)
    //         .ToList();
    //   }
    //   else
    //   {
    //     itemsByFilter = orderDetails.Ordereditems
    //         .Where(i => i.ReadyQuantity > 0)
    //         .ToList();
    //   }

    //   var distinctCategoryIds = itemsByFilter
    //       .Select(i => i.Item.Categoryid)
    //       .Distinct();

    //   foreach (var catId in distinctCategoryIds)
    //   {
    //     if (!categoryOrderIds.ContainsKey(catId))
    //       categoryOrderIds[catId] = new HashSet<int>();

    //     categoryOrderIds[catId].Add(order.Orderid);
    //   }
    // }

    // foreach (var cat in catList)
    // {
    //   if (cat.Categoryid == 0)
    //   {
    //     // "All" → sum all unique order counts
    //     var uniqueOrderIds = new HashSet<int>(
    //         categoryOrderIds.Values.SelectMany(v => v)
    //     );
    //     cat.OrderCount = uniqueOrderIds.Count;
    //   }
    //   else
    //   {
    //     cat.OrderCount = categoryOrderIds.ContainsKey(cat.Categoryid)
    //         ? categoryOrderIds[cat.Categoryid].Count
    //         : 0;
    //   }
    // }

    // return catList;
    int mappedStatus = status == "Inprogress" ? 0 : 1 ;
    return await _categoryRepository.GetKOTCategoryListFromSP(mappedStatus);
  }

}
