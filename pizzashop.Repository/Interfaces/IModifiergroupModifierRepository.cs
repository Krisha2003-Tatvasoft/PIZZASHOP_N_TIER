using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifiergroupModifierRepository 
{
     Task AddNewMapping(ModifierGroupModifier mapping);

     Task<List<ModifierGroupModifier>> GetMappingsByGroupId(int modifierGroupId);

      Task Delete(ModifierGroupModifier mapping);

      Task DeleteMappingsByModifierGroupId(List<ModifierGroupModifier> mappings);

      Task<List<ModifierGroupModifier>> GetMappingsByModifierId(int modifierId);
}
