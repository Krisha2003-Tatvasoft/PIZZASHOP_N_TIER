using Newtonsoft.Json;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class ModifierService : IModifierService
{
  private readonly IModifierRepository _modifierRepository;

  private readonly IUnitRepository _unitRpository;

  private readonly IModifiersGroupRepository _modifiersGropRepository;

  private readonly IModifiergroupModifierRepository _modifiergroupModifierRepository;


  public ModifierService(IModifierRepository modifierRepository, IUnitRepository unitRepository
  , IModifiersGroupRepository modifiersGropRepository, IModifiergroupModifierRepository modifiergroupModifierRepository)
  {
    _modifierRepository = modifierRepository;
    _unitRpository = unitRepository;
    _modifiersGropRepository = modifiersGropRepository;
    _modifiergroupModifierRepository = modifiergroupModifierRepository;
  }

  public async Task<(List<ModifierTable>, int totalMoidifier)> GetModifiersTable(int id, int page, int pageSize, string search)
  {
    var modifierList = await _modifierRepository.GetModifierByMG(id, search);

    int totalMoidifier = modifierList.Count();


    var modifiers = modifierList
     .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(i => new ModifierTable
    {
      Modifierid = i.Modifierid,
      Modifiername = i.Modifiername,
      Rate = i.Rate,
      Quantity = i.Quantity,
      Unitname = i.Unit.Unitname,
      Modifiergroupid = id
    }
    ).ToList();

    return (modifiers, totalMoidifier);
  }


  public async Task<List<ModifierList>> GetModifiersList(int id)
  {
    var modifierList = await _modifierRepository.GetModifierList(id);

    var modifiers = modifierList.Select(i => new ModifierList
    {
      Modifierid = i.Modifierid,
      Modifiername = i.Modifiername,
      Rate = i.Rate
    }
    ).ToList();

    return modifiers;
  }

  public async Task<AddModifier> AddModifier()
  {
    AddModifier model = new AddModifier
    {
      Units = await _unitRpository.GetAllUnitAsync(),
      MGList = await _modifiersGropRepository.GetAllMGAsync()
    };
    return model;
  }

  public async Task<bool> AddModifierPost(int loginId, AddModifier viewmodel)
  {
    if (await _modifierRepository.ModifierExistAsync(viewmodel.Modifiername))
    {
      return false;
    }
    else
    {

      List<int> ModifierGroupIds = JsonConvert.DeserializeObject<List<int>>(viewmodel.ModifierGroupIds);

      Modifier modifier = new Modifier
      {
        Modifiername = viewmodel.Modifiername,
        Modifiergroupid = ModifierGroupIds.FirstOrDefault(),
        Rate = viewmodel.Rate,
        Quantity = viewmodel.Quantity,
        Unitid = viewmodel.Unitid,
        Description = viewmodel.Description,
        Createdby = loginId
      };
      await _modifierRepository.AddNewModifier(modifier);

      foreach (int groupid in ModifierGroupIds)
      {
        ModifierGroupModifier MGM = new ModifierGroupModifier
        {
          ModifierGroupId = groupid,
          ModifierId = modifier.Modifierid
        };
        await _modifiergroupModifierRepository.AddNewMapping(MGM);
      }

      return true;
    }
  }

  public async Task<AddModifier> EditModifier(int id)
  {
    Modifier modifier = await _modifierRepository.ModifierByIdAsync(id);

    var mappedGroups = await _modifiergroupModifierRepository.GetModifierGroupIdsByModifierId(id);


    AddModifier viewmodel = new AddModifier
    {
      Modifierid = modifier.Modifierid,
      Modifiername = modifier.Modifiername,
      SelectedMGIds = mappedGroups,
      Rate = modifier.Rate,
      Quantity = modifier.Quantity,
      Unitid = modifier.Unitid,
      Description = modifier.Description,
      Units = await _unitRpository.GetAllUnitAsync(),
      MGList = await _modifiersGropRepository.GetAllMGAsync()
    };
    return viewmodel;

  }

  public async Task<bool> EditModifierPost(int loginId, AddModifier viewmodel)
  {
    if (await _modifierRepository.ModifierNameExistAtEditAsync(viewmodel.Modifiername,viewmodel.Modifierid))
    {
      return false;
    }
    else
    {
      List<int> newModifierGroupIds = JsonConvert.DeserializeObject<List<int>>(viewmodel.ModifierGroupIds);


      Modifier modifier = await _modifierRepository.ModifierByIdAsync(viewmodel.Modifierid);

      modifier.Modifiername = viewmodel.Modifiername;
      modifier.Modifiergroupid = newModifierGroupIds.FirstOrDefault();
      modifier.Rate = viewmodel.Rate;
      modifier.Quantity = viewmodel.Quantity;
      modifier.Unitid = viewmodel.Unitid;
      modifier.Description = viewmodel.Description;
      modifier.Modifiedby = loginId;

      await _modifierRepository.UpdateModifier(modifier);

      List<ModifierGroupModifier> existingMappings = await _modifiergroupModifierRepository
          .GetMappingsByModifierId(modifier.Modifierid);

      // Extract the existing modifiergroup IDs from mappings
      List<int> existingMGIds = existingMappings.Select(m => m.ModifierGroupId).ToList();

      // Find mappings to remove (previously existed but now removed)
      List<int> toRemove = existingMGIds.Except(newModifierGroupIds).ToList();


      // Find mappings to add (newly added mappings)
      List<int> toAdd = newModifierGroupIds.Except(existingMGIds).ToList();


      foreach (int mgId in toRemove)
      {
        ModifierGroupModifier mgm = existingMappings.FirstOrDefault(m => m.ModifierGroupId == mgId);
        if (mgm != null)
        {
          await _modifiergroupModifierRepository.Delete(mgm);
        }
      }

      foreach (int mgId in toAdd)
      {
        ModifierGroupModifier mgm = new ModifierGroupModifier
        {
          ModifierGroupId = mgId,
          ModifierId = viewmodel.Modifierid
        };
        await _modifiergroupModifierRepository.AddNewMapping(mgm);
      }



      return true;

    }
  }

  public async Task<bool> DeleteModifier(int id, int MGId)
  {
    Modifier modifier = await _modifierRepository.ModifierByIdAsync(id);

    if (modifier == null)
    {
      return false;
    }
    else
    {
      List<ModifierGroupModifier> mapping = await _modifiergroupModifierRepository.GetMappingsByMIdandMGId(id, MGId);

      await _modifiergroupModifierRepository.DeleteMapping(mapping);

      // Count how many groups still have this Modifier
      int groupCount = await _modifiergroupModifierRepository.CountModifierId(id);

      if (groupCount == 0)
      {
        await _modifierRepository.DeleteModifier(modifier);
      }
      return true;
    }

  }

  public async Task<bool> DeleteSelectedModifier(List<int> selectedIds, int MGId)
  {
    if (selectedIds.Count == 0)
    {
      return false;
    }
    var softdeletedIds = new List<int>();
    foreach (var modifierId in selectedIds)
    {
      List<ModifierGroupModifier> mapping = await _modifiergroupModifierRepository.GetMappingsByMIdandMGId(modifierId, MGId);

      await _modifiergroupModifierRepository.DeleteMapping(mapping);

      int groupCount = await _modifiergroupModifierRepository.CountModifierId(modifierId);
      if (groupCount == 0)
      {
        softdeletedIds.Add(modifierId);
      }
    }

    if (softdeletedIds.Count() != 0)
    {
      await _modifierRepository.DeleteSelectedModifier(softdeletedIds);
    }
    return true;
  }

  public async Task<(List<ModifierTable>, int totalExMoidifier)> GetAllModifier(int page, int pageSize, string search)
  {
    var modifierList = await _modifierRepository.GetAllModifier(search);

    int totalExMoidifier = modifierList.Count();

    var modifiers = modifierList
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(i => new ModifierTable
    {
      Modifierid = i.Modifierid,
      Modifiername = i.Modifiername,
      Rate = i.Rate,
      Quantity = i.Quantity,
      Unitname = i.Unit.Unitname
    }
    ).ToList();

    return (modifiers, totalExMoidifier);
  }



}
