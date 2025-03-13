namespace pizzashop.Service.Interfaces;
using pizzashop.Entity.ViewModels;
public interface IModifierService
{
    Task<List<ModifierTable>> GetModifiersTable(int id);
}
