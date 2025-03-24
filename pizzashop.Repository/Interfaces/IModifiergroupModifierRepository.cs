using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifiergroupModifierRepository 
{
     Task AddNewMapping(ModifierGroupModifier mapping);
}
