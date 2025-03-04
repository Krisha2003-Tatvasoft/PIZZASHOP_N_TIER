using pizzashop.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class CityRepository :ICityRepository
{
    private readonly PizzashopContext _context;

        public CityRepository(PizzashopContext context)
        {
            _context = context;
        }

    public async Task<List<SelectListItem>> GetCitiesByStateAsync(int stateId) =>
        await _context.Cities.Where(c => c.Stateid == stateId).Select(c => new SelectListItem { Value = c.Cityid.ToString(), Text = c.Cityname }).ToListAsync();
}
