namespace pizzashop.Service.Interfaces;

using pizzashop.Entity.ViewModels;

using VMModifierGroup = pizzashop.Entity.ViewModels.ModifiersGroup;

public interface IModifiersGroupService
{
    Task<List<VMModifierGroup>> GetMGList();

    Task<bool> AddMGPost(int loginId, AddModifierGroup viewmodel);

    Task<AddModifierGroup> EditMG(int id);

    Task<bool> EditMGPost(int loginId, AddModifierGroup viewmodel);

    Task<bool> DeleteMG(int id);

    Task<bool> SaveOrderMG(List<int> orderIds);
    
}
