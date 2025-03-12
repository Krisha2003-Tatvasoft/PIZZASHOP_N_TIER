using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;


namespace pizzashop.Service.Implementations;

public class ItemService : IItemService
{
  private readonly IItemRepository _itemRepository;
  private readonly ICategoryRepository _categoryRepository;

  private readonly IUnitRepository _unitRpository;

  private readonly IModifiersGroupRepository _modifiersGropRepository;


  public ItemService(IItemRepository itemRepository, ICategoryRepository categoryRepository
  , IUnitRepository unitRepository, IModifiersGroupRepository modifiersGropRepository)
  {
    _itemRepository = itemRepository;
    _categoryRepository = categoryRepository;
    _unitRpository = unitRepository;
    _modifiersGropRepository = modifiersGropRepository;
  }

  public List<ItemTable> GetItemTable(int id)
  {
    var itemList = _itemRepository.GetItemByCat(id);

    var items = itemList.Select(i => new ItemTable
    {
      Itemid = i.Itemid,
      Itemname = i.Itemname,
      Rate = i.Rate,
      Quantity = i.Quantity,
      Unitname = i.Unit.Unitname,
      itemtype = i.itemtype
    }
    ).ToList();

    return items;
  }

  public async Task<AddItem> Additem()
  {

    AddItem model = new AddItem
    {
      Categories = await _categoryRepository.GetAllCatyAsync(),
      Units = await _unitRpository.GetAllUnitAsync(),
      MGList = await _modifiersGropRepository.GetAllMGAsync()
    };
    return model;
  }

  public async Task<bool> AddItemPost(int loginId, AddItem viewmodel)
  {
    if (loginId == null)
    {
      return false;
    }
    Item item = new Item
    {
      Itemname = viewmodel.Itemname,
      Categoryid = viewmodel.Categoryid,
      Rate = viewmodel.Rate,
      Quantity = viewmodel.Quantity,
      Unitid = viewmodel.Unitid,
      Isavailable = viewmodel.Isavailable,
      Taxpercentage = viewmodel.Taxpercentage,
      Isdefaulttax = viewmodel.Isdefaulttax,
      Shortcode = viewmodel.Shortcode,
      Description = viewmodel.Description,
      itemtype = viewmodel.itemtype,
      Createdby = loginId
    };

    _itemRepository.AddNewItemAsync(item);
    return true;

  }

  public async Task<AddItem> EditItem(int id)
  {
    Item item = await _itemRepository.ItemByIdAsync(id);
    AddItem viewmodel = new AddItem
    {
      Itemname = item.Itemname,
      Categoryid = item.Categoryid,
      Rate = item.Rate,
      Quantity = item.Quantity,
      Unitid = item.Unitid,
      Isavailable = item.Isavailable,
      Taxpercentage = item.Taxpercentage,
      Isdefaulttax = item.Isdefaulttax,
      Shortcode = item.Shortcode,
      Description = item.Description,
      itemtype = item.itemtype
    };
    return viewmodel;
  }
}
