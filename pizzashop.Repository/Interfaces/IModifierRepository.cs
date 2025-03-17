using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifierRepository
{
    Task<List<Modifier>> GetModifierByMG(int id,string search);

    Task<List<Modifier>> GetModifierList(int id);

    Task AddNewModifier(Modifier modifier);

    Task<Modifier> ModifierByIdAsync(int id);

    Task UpdateModifier(Modifier modifier);

     Task DeleteModifier(Modifier modifier);

     Task DeleteSelectedModifier(List<int> SelectedIds);
     
    
}
