using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class ModifiersGroupRepository : IModifiersGroupRepository
{
    private readonly PizzashopContext _context;

    public ModifiersGroupRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Modifiergroup>> AllModifiersGroup()
    {
        return await _context.Modifiergroups
        .Where(c=>c.Isdeleted == false).OrderBy(c=> c.Modifiergroupid).ToListAsync();
    }

    public async Task<List<SelectListItem>> GetAllMGAsync() =>
      await _context.Modifiergroups
      .Where(c=>c.Isdeleted == false).Select
      (c => new SelectListItem
      { Value = c.Modifiergroupid.ToString(), Text = c.Modifiergroupname })
      .ToListAsync();

    public async Task AddNewMG(Modifiergroup modifiergroup)
    {
        _context.Modifiergroups.Add(modifiergroup);
        await _context.SaveChangesAsync();
    }

     public async Task<Modifiergroup> MGByIdAsync(int id)
    {
        return await _context.Modifiergroups.FirstOrDefaultAsync(u => u.Modifiergroupid == id);
    }

     public async Task UpdateMG(Modifiergroup modifiergroup)
    {
        _context.Modifiergroups.Update(modifiergroup);
        await _context.SaveChangesAsync();
    }


   public async Task DeleteMG(Modifiergroup modifiergroup)
    {
        modifiergroup.Isdeleted = true;
        _context.Modifiergroups.Update(modifiergroup);
        await _context.SaveChangesAsync();
    }




}
