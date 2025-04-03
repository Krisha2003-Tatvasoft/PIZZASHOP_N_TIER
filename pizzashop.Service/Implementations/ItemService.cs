using Microsoft.AspNetCore.Http;
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

  private readonly IItemmodifiergroupmapRepository _itemmodifiergroupmapRepository;

  private readonly IModifierRepository _modifierRepository;

  private readonly IFileService _fileService;

  private readonly IHttpContextAccessor _httpContextAccessor;


  public ItemService(IItemRepository itemRepository, ICategoryRepository categoryRepository
  , IUnitRepository unitRepository, IModifiersGroupRepository modifiersGropRepository
  , IItemmodifiergroupmapRepository itemmodifiergroupmapRepository, IHttpContextAccessor httpContextAccessor,
   IModifierRepository modifierRepository, IFileService fileService)
  {
    _itemRepository = itemRepository;
    _categoryRepository = categoryRepository;
    _unitRpository = unitRepository;
    _modifiersGropRepository = modifiersGropRepository;
    _itemmodifiergroupmapRepository = itemmodifiergroupmapRepository;
    _modifierRepository = modifierRepository;
    _fileService = fileService;
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<(List<ItemTable>, int totalitem)> GetItemTable(int id, int page, int pageSize, string search)
  {
    var itemList = await _itemRepository.GetItemByCat(id, search);

    int totalitem = itemList.Count();

    var items = itemList
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(i => new ItemTable
    {
      Categoryid = id,
      Itemid = i.Itemid,
      Itemname = i.Itemname,
      Rate = i.Rate,
      Quantity = i.Quantity,
      Unitname = i.Unit.Unitname,
      itemtype = i.itemtype,
      Isavailable = i.Isavailable,
      Itemimg = i.Itemimg != null
            ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/{i.Itemimg}"
            : null
    }
    ).ToList();

    return (items, totalitem);
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
    string uniqueFileName = null;
    if (viewmodel.ItemPicture != null)
    {
      uniqueFileName = await _fileService.UploadFileAsync(viewmodel.ItemPicture, "uploads");
    }

    if (await _itemRepository.ItemExistAsync(viewmodel.Itemname))
    {
      return false;
    }
    // if (viewmodel.ModifierGroups == null || !viewmodel.ModifierGroups.Any())
    // {
    //   Console.WriteLine("ModifierGroups is either null or empty, skipping mapping.");
    //   return false;
    // }

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
      Createdby = loginId,
      Itemimg = uniqueFileName
    };

    await _itemRepository.AddNewItemAsync(item);

    foreach (var group in viewmodel.ModifierGroups)
    {

      var newMapping = new Itemmodifiergroupmap
      {

        Itemid = item.Itemid,
        Modifiergroupid = group.Modifiergroupid,
        Minselectionrequired = group.Minselectionrequired,
        Maxselectionallowed = group.Maxselectionallowed
      };

      await _itemmodifiergroupmapRepository.AddNewMapping(newMapping);
    }

    return true;

  }

  public async Task<AddItem> EditItem(int id)
  {
    Item item = await _itemRepository.ItemByIdAsync(id);
    var selectedMG = await _itemmodifiergroupmapRepository.GetMGMByitemid(id);

    var ModifierGroups = new List<IMGMviewmodel>();

    foreach (var img in selectedMG)
    {
      var modifiers = await _modifierRepository.GetModifierList(img.Modifiergroupid);

      ModifierGroups.Add(new IMGMviewmodel
      {
        Itemmodifiergroupid = img.Itemmodifiergroupid,
        Itemid = img.Itemid,
        Modifiergroupid = img.Modifiergroupid,
        Minselectionrequired = img.Minselectionrequired,
        Maxselectionallowed = img.Maxselectionallowed,
        modifiers = modifiers,
        Modifiername = img.Modifiergroup.Modifiergroupname
      });

    }


    AddItem viewmodel = new AddItem
    {
      Itemid = item.Itemid,
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
      itemtype = item.itemtype,
      Categories = await _categoryRepository.GetAllCatyAsync(),
      Units = await _unitRpository.GetAllUnitAsync(),
      MGList = await _modifiersGropRepository.GetAllMGAsync(),
      ModifierGroups = ModifierGroups,
      selectedMGList = selectedMG.Select(m => m.Modifiergroupid).ToList(),
      Itemimg = item.Itemimg != null
            ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/{item.Itemimg}"
            : null,
    };
    return viewmodel;
  }

  public async Task<bool> EditItemPost(int loginid, AddItem viewmodel)
  {
    Item item = await _itemRepository.ItemByIdAsync(viewmodel.Itemid);
    string uniqueFileName = null;
    if (viewmodel.ItemPicture != null)
    {
      if (!string.IsNullOrEmpty(item.Itemimg))
      {
        _fileService.DeleteFile(item.Itemimg);
      }
      uniqueFileName = await _fileService.UploadFileAsync(viewmodel.ItemPicture, "uploads");
    }
    else
    {
   
      uniqueFileName = item.Itemimg;
    }
    if (await _itemRepository.ItemNameExistAtEditAsync(viewmodel.Itemname, viewmodel.Itemid))
    {
      return false;
    }


    item.Itemname = viewmodel.Itemname;
    item.Categoryid = viewmodel.Categoryid;
    item.Rate = viewmodel.Rate;
    item.Quantity = viewmodel.Quantity;
    item.Unitid = viewmodel.Unitid;
    item.Isavailable = viewmodel.Isavailable;
    item.Taxpercentage = viewmodel.Taxpercentage;
    item.Isdefaulttax = viewmodel.Isdefaulttax;
    item.Shortcode = viewmodel.Shortcode;
    item.Description = viewmodel.Description;
    item.itemtype = viewmodel.itemtype;
    item.Modifiedby = loginid;
    item.Itemimg = uniqueFileName;

    await _itemRepository.UpdateItem(item);

    // Fetch all existing mappings once
    var existingMappings = await _itemmodifiergroupmapRepository.GetMGMByitemid(viewmodel.Itemid);
    var existingGroupIds = existingMappings.Select(m => m.Modifiergroupid).ToList();


    var newGroupIds = viewmodel.ModifierGroups.Select(m => m.Modifiergroupid).ToList();

    // ✅ Fix: Remove only groups that are explicitly missing in the new list
    var groupsToRemove = existingMappings
        .Where(m => !newGroupIds.Contains(m.Modifiergroupid))
        .ToList();

    foreach (var group in groupsToRemove)
    {
      await _itemmodifiergroupmapRepository.DeleteMapping(group);
    }

    // ✅ Fix: Add only new groups that are not in the existing list
    var groupsToAdd = viewmodel.ModifierGroups
        .Where(m => !existingGroupIds.Contains(m.Modifiergroupid))
        .ToList();

    foreach (var group in groupsToAdd)
    {
      var newMapping = new Itemmodifiergroupmap
      {
        Itemid = item.Itemid,
        Modifiergroupid = group.Modifiergroupid,
        Minselectionrequired = group.Minselectionrequired,
        Maxselectionallowed = group.Maxselectionallowed
      };

      await _itemmodifiergroupmapRepository.AddNewMapping(newMapping);
    }


    foreach (var existingMapping in existingMappings)
    {
      var updatedGroup = viewmodel.ModifierGroups.FirstOrDefault(m => m.Modifiergroupid == existingMapping.Modifiergroupid);
      if (updatedGroup != null)
      {
        if (existingMapping.Minselectionrequired != updatedGroup.Minselectionrequired ||
            existingMapping.Maxselectionallowed != updatedGroup.Maxselectionallowed)
        {
          existingMapping.Minselectionrequired = updatedGroup.Minselectionrequired;
          existingMapping.Maxselectionallowed = updatedGroup.Maxselectionallowed;
          await _itemmodifiergroupmapRepository.UpdateMapping(existingMapping);
        }
      }
    }
    return true;

  }


  public async Task<bool> DeleteItem(int id)
  {
    Item item = await _itemRepository.ItemByIdAsync(id);
    if (item == null)
    {
      return false;
    }
    else
    {
      await _itemRepository.DeleteItem(item);
      await _itemmodifiergroupmapRepository.DeleteMappingByItem(id);
      return true;
    }

  }

  public async Task<bool> DeleteSelectedItem(List<int> selectedIds)
  {
    if (selectedIds.Count == 0)
    {
      return false;
    }

    await _itemRepository.DeleteSelected(selectedIds);
    foreach (var id in selectedIds)
    {
      await _itemmodifiergroupmapRepository.DeleteMappingByItem(id);
    }
    return true;
  }

    public async Task<bool> UpdateAvailable(int loginId, int id ,bool available)
    {
        if (loginId == null)
        {
            return false;
        }
        else
        {
            Item item = await _itemRepository.ItemByIdAsync(id);

            item.Isavailable = available;
            item.Modifiedby= loginId;

            await _itemRepository.UpdateItem(item);
            return true;
        }
    }


}
