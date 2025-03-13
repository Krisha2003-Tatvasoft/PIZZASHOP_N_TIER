using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class ModifierService:IModifierService
{
    private readonly IModifierRepository _modifierRepository;

    public ModifierService(IModifierRepository modifierRepository)
    {
        _modifierRepository = modifierRepository;
    }

    public async Task<List<ModifierTable>> GetModifiersTable(int id)
    {
        var modifierList = await _modifierRepository.GetModifierByMG(id);

        var modifiers = modifierList.Select(i => new ModifierTable
        {
            Modifierid = i.Modifierid,
            Modifiername = i.Modifiername,
            Rate = i.Rate,
            Quantity = i.Quantity,
            Unitname = i.Unit.Unitname
        }
        ).ToList();

        return modifiers;
    }
}
