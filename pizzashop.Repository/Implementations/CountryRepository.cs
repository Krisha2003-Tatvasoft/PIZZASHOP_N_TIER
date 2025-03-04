using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class CountryRepository :ICountryRepository
{
      private readonly PizzashopContext _context;

        public CountryRepository(PizzashopContext context)
        {
            _context = context;
        }

     public async Task<List<SelectListItem>> GetAllCountryAsync() =>
        await _context.Countries.Select
        (c => new SelectListItem 
        { Value = c.Countryid.ToString(), Text = c.Countryname })
        .ToListAsync();
}
