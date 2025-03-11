using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class UnitRepository :IUnitRepository
{
      private readonly PizzashopContext _context;

        public UnitRepository(PizzashopContext context)
        {
            _context = context;
        }

      public async Task<List<SelectListItem>> GetAllUnitAsync() =>
        await _context.Units.Select
        (c => new SelectListItem 
        { Value = c.Unitid.ToString(), Text = c.Unitname })
        .ToListAsync();
}
