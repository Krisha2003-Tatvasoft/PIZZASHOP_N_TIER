using Microsoft.AspNetCore.JsonPatch.Internal;
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
        return await _context.Taxes
        .Where(u => u.Isdeleted == false && u.Taxid != 0)
        .OrderBy(u => u.Taxid)
        .Where(u => string.IsNullOrEmpty(lowerSearch) ||
                            u.Taxname.ToLower().Contains(lowerSearch)).ToListAsync();
    }

    public async Task<bool> TaxesNameExist(string taxname)
    {
        return await _context.Taxes.AnyAsync(s => s.Taxname.ToLower().Trim() == taxname.ToLower().Trim() && s.Isdeleted == false);
    }

    public async Task AddNewTax(Taxis tax)
    {
        _context.Taxes.Add(tax);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> TaxNameExistInEdit(string taxname, int taxid)
    {
        return await _context.Taxes.AnyAsync(s => s.Taxname.ToLower().Trim() == taxname.ToLower().Trim()
        && s.Taxid != taxid && s.Isdeleted == false);
    }

    public async Task UpdateTax(Taxis tax)
    {
        _context.Taxes.Update(tax);
        await _context.SaveChangesAsync();
    }

    public async Task<Taxis> TaxByIdAsync(int id)
    {
        return await _context.Taxes.FirstOrDefaultAsync(u => u.Taxid == id);
    }


    public async Task DeleteTax(Taxis tax)
    {
        tax.Isdeleted = true;
        _context.Taxes.Update(tax);
        await _context.SaveChangesAsync();
    }


    public async Task<List<Taxis>> GetAllTaxEnabled()
    {
        return await _context.Taxes.Where(t => t.Isenabled == true && t.Isdeleted == false).ToListAsync();
    }




}
