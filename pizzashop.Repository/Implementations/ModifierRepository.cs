using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class ModifierRepository :IModifierRepository
{
    private readonly PizzashopContext _context;

    public ModifierRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Modifier>> GetModifierByMG(int id)
    {
        return await _context.Modifiers
        .Include(u => u.Unit)
        .Where(c => c.Modifiergroupid == id).ToListAsync();
    }

    public async Task<List<Modifier>> GetModifierList(int id)
    {
        return await _context.Modifiers
        .Where(c => c.Modifiergroupid == id).ToListAsync();
    }


}
