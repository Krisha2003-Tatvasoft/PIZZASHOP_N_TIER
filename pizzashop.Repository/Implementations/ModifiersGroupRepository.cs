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
        return await  _context.Modifiergroups.ToListAsync();
    }

      public async Task<List<SelectListItem>> GetAllMGAsync() =>
        await _context.Modifiergroups.Select
        (c => new SelectListItem 
        { Value = c.Modifiergroupid.ToString(), Text = c.Modifiergroupname })
        .ToListAsync();

}
