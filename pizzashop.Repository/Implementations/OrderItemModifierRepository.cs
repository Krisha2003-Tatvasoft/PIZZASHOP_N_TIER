using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class OrderItemModifierRepository : IOrderItemModifierRepository
{
    private readonly PizzashopContext _context;

    public OrderItemModifierRepository(PizzashopContext context)
    {
        _context = context;
    }

    // Get Modifiers for a specific OrderItem
    public async Task<List<Ordereditemmodifer>> GetModifiersByOrderItemIdAsync(int orderItemId)
    {
        return await _context.Ordereditemmodifers
            .Where(oim => oim.Ordereditemid == orderItemId)
            .ToListAsync();
    }

    // Add Modifier to Order Item
    public async Task AddOrderItemModifierAsync(Ordereditemmodifer orderItemModifier)
    {
        _context.Ordereditemmodifers.Add(orderItemModifier);
        await _context.SaveChangesAsync();
    }

    // Delete Modifiers for an OrderItem
    public async Task DeleteModifiersByOrderItemIdAsync(int orderItemId)
    {
        var orderModifiers = await _context.Ordereditemmodifers
            .Where(oim => oim.Ordereditemid == orderItemId)
            .ToListAsync();

        _context.Ordereditemmodifers.RemoveRange(orderModifiers);
        await _context.SaveChangesAsync();
    }


}
