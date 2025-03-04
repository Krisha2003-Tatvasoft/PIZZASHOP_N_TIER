using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class StateRepository :IStateRepository
{
     private readonly PizzashopContext _context;

        public StateRepository(PizzashopContext context)
        {
            _context = context;
        }

       public async Task<List<SelectListItem>> GetStatesByCountryAsync(int countryId) =>
        await _context.States.Where(s => s.Countryid == countryId).Select(s => new SelectListItem 
        { Value = s.Stateid.ToString(), 
        Text = s.Statename }).ToListAsync();

}
     