using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly PizzashopContext _context;

    public OrderItemRepository(PizzashopContext context)
    {
        _context = context;
    }

    // Get Order Items by Order ID
    public async Task<List<Ordereditem>> GetOrderItemsByOrderIdAsync(int orderId)
    {
        return await _context.Ordereditems
            .Where(oi => oi.Orderid == orderId)
            .ToListAsync();
    }

    // Add new Order Item
    public async Task<Ordereditem> AddOrderItemAsync(Ordereditem orderItem)
    {
        _context.Ordereditems.Add(orderItem);
        await _context.SaveChangesAsync();
        return orderItem;
    }

    // Update existing Order Item
    public async Task UpdateOrderItemAsync(Ordereditem orderItem)
    {
        _context.Ordereditems.Update(orderItem);
        await _context.SaveChangesAsync();
    }

    // Delete Order Item
    public async Task DeleteOrderItemAsync(int orderItemId)
    {
        var orderItem = await _context.Ordereditems.FindAsync(orderItemId);
        if (orderItem != null)
        {
            _context.Ordereditems.Remove(orderItem);
            await _context.SaveChangesAsync();
        }
    }


}
