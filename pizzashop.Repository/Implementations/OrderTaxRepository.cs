using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class OrderTaxRepository : IOrderTaxRepository
{
    private readonly PizzashopContext _context;

    public OrderTaxRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Ordertaxmapping>> GetTaxesByOrderIdAsync(int orderId)
    {
        return await _context.Ordertaxmappings.Where(t => t.Orderid == orderId).ToListAsync();
    }

    public async Task AddAsync(Ordertaxmapping orderTax)
    {
        _context.Ordertaxmappings.AddAsync(orderTax);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ordertaxmapping orderTax)
    {
        _context.Ordertaxmappings.Update(orderTax);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Ordertaxmapping orderTax)
    {
        _context.Ordertaxmappings.Remove(orderTax);
        await _context.SaveChangesAsync();
    }
}
