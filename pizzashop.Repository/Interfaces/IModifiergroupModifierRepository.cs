using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifiergroupModifierRepository 
{
     Task AddNewMapping(ModifierGroupModifier mapping);

     Task<List<ModifierGroupModifier>> GetMappingsByGroupId(int modifierGroupId);

      Task Delete(ModifierGroupModifier mapping);

      Task DeleteMapping(List<ModifierGroupModifier> mappings);

      Task<List<ModifierGroupModifier>> GetMappingsByModifierId(int modifierId);

      Task<List<int>> GetModifierGroupIdsByModifierId(int modifierId);

      Task<int> CountModifierId(int modifierid);

      Task<List<ModifierGroupModifier>> GetMappingsByMIdandMGId(int modifierId, int MGId);
}
