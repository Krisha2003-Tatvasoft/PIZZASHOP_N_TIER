using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class ModifiergroupModifierRepository : IModifiergroupModifierRepository
{
    private readonly PizzashopContext _context;

    public ModifiergroupModifierRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddNewMapping(ModifierGroupModifier mapping)
    {
        _context.ModifierGroupModifiers.Add(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ModifierGroupModifier>> GetMappingsByGroupId(int modifierGroupId)
    {
        return await _context.ModifierGroupModifiers
            .Where(mgm => mgm.ModifierGroupId == modifierGroupId)
            .ToListAsync();
    }

    public async Task Delete(ModifierGroupModifier mapping)
    {
        _context.ModifierGroupModifiers.Remove(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMapping(List<ModifierGroupModifier> mappings)
    {
        _context.ModifierGroupModifiers.RemoveRange(mappings);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ModifierGroupModifier>> GetMappingsByModifierId(int modifierId)
    {
        return await _context.ModifierGroupModifiers
        .Where(mg => mg.ModifierId == modifierId)
        .ToListAsync();
    }

    public async Task<List<int>> GetModifierGroupIdsByModifierId(int modifierId)
    {
        return await _context.ModifierGroupModifiers
                             .Where(mgm => mgm.ModifierId == modifierId)
                             .Select(mgm => mgm.ModifierGroupId)
                             .ToListAsync();
    }

    public async Task<int> CountModifierId(int modifierid)
    {
        return await _context.ModifierGroupModifiers
        .CountAsync(mgm => mgm.ModifierId == modifierid);
    }


    public async Task<List<ModifierGroupModifier>> GetMappingsByMIdandMGId(int modifierId, int MGId)
    {
        return await _context.ModifierGroupModifiers
        .Where(mg => mg.ModifierId == modifierId && mg.ModifierGroupId == MGId)
        .ToListAsync();
    }





}
