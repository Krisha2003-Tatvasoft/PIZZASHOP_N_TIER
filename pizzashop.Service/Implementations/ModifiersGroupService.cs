using Newtonsoft.Json;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using VMModifierGroup = pizzashop.Entity.ViewModels.ModifiersGroup;

namespace pizzashop.Service.Implementations;

public class ModifiersGroupService : IModifiersGroupService
{
    private readonly IModifiersGroupRepository _modifierGroupRepository;

    private readonly IModifiergroupModifierRepository _modifiergroupModifierRepository;

    public ModifiersGroupService(IModifiersGroupRepository modifiersGroupRepository,IModifiergroupModifierRepository modifiergroupModifierRepository)
    {
        _modifierGroupRepository = modifiersGroupRepository;
        _modifiergroupModifierRepository = modifiergroupModifierRepository;
    }
    public async Task<List<VMModifierGroup>> GetMGList()
    {
        var category = await _modifierGroupRepository.AllModifiersGroup();

        var MGList = category.Select(c => new VMModifierGroup
        {
            Modifiergroupid = c.Modifiergroupid,
            Modifiergroupname = c.Modifiergroupname,
            Description = c.Description,
            Createdby = c.Createdby,
            Modifiedby = c.Modifiedby
        }).ToList();

        return MGList;
    }

    public async Task<bool> AddMGPost(int loginId, AddModifierGroup viewmodel)
    {
        if (loginId == null)
        {
            return false;
        }
        else
        {
            Modifiergroup Mg = new Modifiergroup
            {
                Modifiergroupname = viewmodel.Modifiergroupname,
                Description = viewmodel.Description,
                Createdby = loginId
            };
            await _modifierGroupRepository.AddNewMG(Mg);

            List<int> modifiersIds = JsonConvert.DeserializeObject<List<int>>(viewmodel.selectedModifier);

            foreach(int id in modifiersIds)
            {
               ModifierGroupModifier MGM = new ModifierGroupModifier{
                  ModifierGroupId=Mg.Modifiergroupid,
                  ModifierId=id
               };
               await _modifiergroupModifierRepository.AddNewMapping(MGM);
            }
            return true;
        }
    }

    public async Task<AddModifierGroup> EditMG(int id)
    {
        Modifiergroup mg = await _modifierGroupRepository.MGByIdAsync(id);
        AddModifierGroup viewmodel = new AddModifierGroup
        {
            Modifiergroupid = id,
            Modifiergroupname = mg.Modifiergroupname,
            Description = mg.Description
        };
        return viewmodel;
    }

    public async Task<bool> EditMGPost(int loginId, AddModifierGroup viewmodel)
    {
        if (loginId == null)
        {
            return false;
        }
        else
        {
            Modifiergroup mg = await _modifierGroupRepository.MGByIdAsync(viewmodel.Modifiergroupid);

            mg.Modifiergroupname = viewmodel.Modifiergroupname;
            mg.Description = viewmodel.Description;
            mg.Modifiedby = loginId;

            await _modifierGroupRepository.UpdateMG(mg);
            return true;

        }
    }


    public async Task<bool> DeleteMG(int id)
    {
        Modifiergroup mg = await _modifierGroupRepository.MGByIdAsync(id);
        if (mg == null)
        {
            return false;
        }
        else
        {
            await _modifierGroupRepository.DeleteMG(mg);
            return true;
        }

    }

}
