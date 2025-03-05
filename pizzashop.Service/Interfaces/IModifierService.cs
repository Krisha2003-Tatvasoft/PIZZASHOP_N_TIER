namespace pizzashop.Service.Interfaces;
using pizzashop.Entity.ViewModels;
public interface IModifierService
{
    List<ModifierTable> GetModifiersTable(int id);
}
