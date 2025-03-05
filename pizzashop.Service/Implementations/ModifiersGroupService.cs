using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using VMModifierGroup = pizzashop.Entity.ViewModels.ModifiersGroup;

namespace pizzashop.Service.Implementations;

public class ModifiersGroupService : IModifiersGroupService
{
    private readonly IModifiersGroupRepository _modifierGroupRepository;

    public ModifiersGroupService(IModifiersGroupRepository modifiersGroupRepository)
    {
        _modifierGroupRepository = modifiersGroupRepository;
    }
    public List<VMModifierGroup> GetMGList()
    {
        var category = _modifierGroupRepository.AllModifiersGroup();

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

}
