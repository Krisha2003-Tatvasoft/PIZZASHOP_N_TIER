using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class ModifierRepository : IModifierRepository
{
    private readonly PizzashopContext _context;

    public ModifierRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Modifier>> GetModifierByMG(int id, string search)
    {
        string lowerSearch = search.ToLower();
        return await _context.Modifiers
        .Include(u => u.Unit)
        .Include(mgm => mgm.ModifierGroupModifier)
        .Where(m => m.Isdeleted == false)
         .Where(m => m.ModifierGroupModifier.Any(mgm => mgm.ModifierGroupId == id))
          .Where(c => string.IsNullOrEmpty(lowerSearch) ||
           c.Modifiername.ToLower().Contains(lowerSearch) ||
           c.Unit.Unitname.ToLower().Contains(lowerSearch) ||
           c.Quantity.ToString().ToLower().Contains(lowerSearch) ||
          c.Rate.ToString().ToLower().Contains(lowerSearch))
         .OrderBy(c => c.Modifierid).ToListAsync();
    }

    public async Task<List<Modifier>> GetModifierList(int id)
    {
        return await _context.Modifiers
        .Where(c => c.Modifiergroupid == id && c.Isdeleted == false).ToListAsync();
    }

    public async Task AddNewModifier(Modifier modifier)
    {
        _context.Modifiers.Add(modifier);
        await _context.SaveChangesAsync();
    }

    public async Task<Modifier> ModifierByIdAsync(int id)
    {
        return await _context.Modifiers.FirstOrDefaultAsync(u => u.Modifierid == id);
    }

    public async Task UpdateModifier(Modifier modifier)
    {
        _context.Modifiers.Update(modifier);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteModifier(Modifier modifier)
    {
        modifier.Isdeleted = true;
        _context.Modifiers.Update(modifier);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSelectedModifier(List<int> SelectedIds)
    {
        await _context.Modifiers
       .Where(i => SelectedIds.Contains(i.Modifierid))
       .ExecuteUpdateAsync(s => s.SetProperty(i => i.Isdeleted, true));

        await _context.SaveChangesAsync();
    }

    public async Task<List<Modifier>> GetAllModifier(string search)
    {
        string lowerSearch = search.ToLower();

        return await _context.Modifiers.Include(u => u.Unit)
        .Where(c => c.Isdeleted == false)
          .Where(c => string.IsNullOrEmpty(lowerSearch) ||
         c.Modifiername.ToLower().Contains(lowerSearch) ||
           c.Unit.Unitname.ToLower().Contains(lowerSearch) ||
           c.Quantity.ToString().ToLower().Contains(lowerSearch) ||
          c.Rate.ToString().ToLower().Contains(lowerSearch))
         .OrderBy(c => c.Modifierid).ToListAsync();
    }

    public async Task<List<Modifier>> GetModifiersByIds(List<int> modifierIds)
    {
        return await _context.Modifiers
            .Where(m => modifierIds.Contains(m.Modifierid))
            .ToListAsync();
    }

    public async Task<bool> ModifierExistAsync(string Modifiername)
    {
        return await _context.Modifiers.AnyAsync(c => c.Modifiername.ToLower() == Modifiername.ToLower() && c.Isdeleted == false);
    }

    public async Task<bool> ModifierNameExistAtEditAsync(string Modifiername, int id)
    {
        return await _context.Modifiers.AnyAsync(c => c.Modifiername.ToLower() == Modifiername.ToLower() && c.Modifierid != id 
        && c.Isdeleted == false);
    }

}
