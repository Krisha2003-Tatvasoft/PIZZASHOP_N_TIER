namespace pizzashop.Service.Interfaces;
using pizzashop.Entity.ViewModels;
public interface IModifierService
{
   Task<(List<ModifierTable> , int totalMoidifier)> GetModifiersTable(int id,int page, int pageSize, string search);

    Task<List<ModifierList>> GetModifiersList(int id);

    Task<AddModifier> AddModifier();

    Task<bool> AddModifierPost(int loginId,AddModifier viewmodel);

    Task<AddModifier> EditModifier(int id);

     Task<bool> EditModifierPost(int loginId, AddModifier viewmodel);

     Task<bool> DeleteModifier(int id);

     Task<bool> DeleteSelectedModifier(List<int> selectedIds);

    Task<List<ModifierTable>> GetAllModifier(page, pageSize, search);
    
}
