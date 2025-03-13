namespace pizzashop.Service.Interfaces;
using VMModifierGroup = pizzashop.Entity.ViewModels.ModifiersGroup;

public interface IModifiersGroupService
{
    Task<List<VMModifierGroup>> GetMGList();
}
