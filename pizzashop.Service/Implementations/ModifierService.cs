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


  public ModifierService(IModifierRepository modifierRepository, IUnitRepository unitRepository
  , IModifiersGroupRepository modifiersGropRepository)
  {
    _modifierRepository = modifierRepository;
    _unitRpository = unitRepository;
    _modifiersGropRepository = modifiersGropRepository;
  }

  public async Task<(List<ModifierTable> , int totalMoidifier)> GetModifiersTable(int id,int page, int pageSize, string search)
  {
    var modifierList = await _modifierRepository.GetModifierByMG(id,search);

     int totalMoidifier =  modifierList.Count();


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

    return (modifiers , totalMoidifier);
  }


  public async Task<List<ModifierList>> GetModifiersList(int id)
  {
    var modifierList = await _modifierRepository.GetModifierList(id);

    var modifiers = modifierList.Select(i => new ModifierList
    {
      Modifierid = i.Modifierid,
      Modifiername = i.Modifiername,
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
    if (loginId == null)
    {
      return false;
    }
    else
    {
      Modifier modifier = new Modifier
      {
        Modifiername = viewmodel.Modifiername,
        Modifiergroupid = viewmodel.Modifiergroupid,
        Rate = viewmodel.Rate,
        Quantity = viewmodel.Quantity,
        Unitid = viewmodel.Unitid,
        Description = viewmodel.Description,
        Createdby = loginId
      };
      await _modifierRepository.AddNewModifier(modifier);
      return true;
    }
  }

  public async Task<AddModifier> EditModifier(int id)
  {
    Modifier modifier = await _modifierRepository.ModifierByIdAsync(id);
    AddModifier viewmodel = new AddModifier
    {
      Modifierid = modifier.Modifierid,
      Modifiername = modifier.Modifiername,
      Modifiergroupid = modifier.Modifiergroupid,
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
    if (loginId == null)
    {
      return false;
    }
    else
    {
      Modifier modifier = await _modifierRepository.ModifierByIdAsync(viewmodel.Modifierid);

      modifier.Modifiername = viewmodel.Modifiername;
      modifier.Modifiergroupid = viewmodel.Modifiergroupid;
      modifier.Rate = viewmodel.Rate;
      modifier.Quantity = viewmodel.Quantity;
      modifier.Unitid = viewmodel.Unitid;
      modifier.Description = viewmodel.Description;
      modifier.Modifiedby = loginId;

      await _modifierRepository.UpdateModifier(modifier);
      return true;

    }
  }

  public async Task<bool> DeleteModifier(int id)
  {
    Modifier modifier = await _modifierRepository.ModifierByIdAsync(id);

    if (modifier == null)
    {
      return false;
    }
    else
    {
      await _modifierRepository.DeleteModifier(modifier);
      return true;
    }

  }

    public async Task<bool> DeleteSelectedModifier(List<int> selectedIds)
  {
    if (selectedIds.Count == 0)
    {
      return false;
    }

    await _modifierRepository.DeleteSelectedModifier(selectedIds);
    return true;
  }





}
