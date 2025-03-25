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

    private readonly IModifierRepository _modifierRepository;

    public ModifiersGroupService(IModifiersGroupRepository modifiersGroupRepository, IModifiergroupModifierRepository modifiergroupModifierRepository,
    IModifierRepository modifierRepository)
    {
        _modifierGroupRepository = modifiersGroupRepository;
        _modifiergroupModifierRepository = modifiergroupModifierRepository;
        _modifierRepository = modifierRepository;
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

            foreach (int id in modifiersIds)
            {
                ModifierGroupModifier MGM = new ModifierGroupModifier
                {
                    ModifierGroupId = Mg.Modifiergroupid,
                    ModifierId = id
                };
                await _modifiergroupModifierRepository.AddNewMapping(MGM);
            }
            return true;
        }
    }

    public async Task<AddModifierGroup> EditMG(int id)
    {
        Modifiergroup mg = await _modifierGroupRepository.MGByIdAsync(id);

        List<ModifierGroupModifier> existingMappings = await _modifiergroupModifierRepository.GetMappingsByGroupId(id);

        List<Modifier> selectedModifiers = await _modifierRepository.GetModifiersByIds(existingMappings.Select(m => m.ModifierId).ToList());
        AddModifierGroup viewmodel = new AddModifierGroup
        {
            Modifiergroupid = id,
            Modifiergroupname = mg.Modifiergroupname,
            Description = mg.Description,
            SelectedModifiers = selectedModifiers.Select(m => new ModifierList
            {
                Modifierid = m.Modifierid,
                Modifiername = m.Modifiername
            }).ToList()

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

            List<ModifierGroupModifier> existingMappings = await _modifiergroupModifierRepository
            .GetMappingsByGroupId(viewmodel.Modifiergroupid);

            // Get the new list of selected modifier IDs from the request
            List<int> newModifierIds = JsonConvert.DeserializeObject<List<int>>(viewmodel.selectedModifier);

            // Extract the existing modifier IDs from mappings
            List<int> existingModifierIds = existingMappings.Select(m => m.ModifierId).ToList();

            // *Find Modifiers to Remove (Existing - New)*
            List<int> modifiersToRemove = existingModifierIds.Except(newModifierIds).ToList();

            // *Find Modifiers to Add (New - Existing)*
            List<int> modifiersToAdd = newModifierIds.Except(existingModifierIds).ToList();

            // *Remove Unselected Modifiers*
            foreach (int modifierId in modifiersToRemove)
            {
                ModifierGroupModifier mgm = existingMappings.FirstOrDefault(m => m.ModifierId == modifierId);
                if (mgm != null)
                {
                    await _modifiergroupModifierRepository.Delete(mgm);
                }
            }

            // *Add Newly Selected Modifiers*
            foreach (int modifierId in modifiersToAdd)
            {
                ModifierGroupModifier mgm = new ModifierGroupModifier
                {
                    ModifierGroupId = viewmodel.Modifiergroupid,
                    ModifierId = modifierId
                };
                await _modifiergroupModifierRepository.AddNewMapping(mgm);
            }
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

            var mappings = await _modifiergroupModifierRepository.GetMappingsByGroupId(id);
             await _modifiergroupModifierRepository.DeleteMappingsByModifierGroupId(mappings);
            return true;
        }
    }

}
