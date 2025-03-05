namespace pizzashop.Service.Interfaces;
using VMModifierGroup = pizzashop.Entity.ViewModels.ModifiersGroup;

public interface IModifiersGroupService
{
    List<VMModifierGroup> GetMGList();
}
