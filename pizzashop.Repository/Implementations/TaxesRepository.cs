using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;
namespace pizzashop.Repository.Implementations;

public class TaxesRepository : ITaxesRepository
{
    private readonly PizzashopContext _context;

    public TaxesRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Taxis>> GetTaxTableAsync(string search)
    {
        string lowerSearch = search.ToLower();
        return await  _context.Taxes
        .Where(u => u.Isdeleted == false)
        .Where(u => string.IsNullOrEmpty(lowerSearch) ||
                            u.Taxname.ToLower().Contains(lowerSearch)).ToListAsync();
    }

}
