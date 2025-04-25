using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IOrderItemRepository
{
    Task<List<Ordereditem>> GetOrderItemsByOrderIdAsync(int orderId);

    Task<Ordereditem> AddOrderItemAsync(Ordereditem orderItem);

    Task UpdateOrderItemAsync(Ordereditem orderItem);

    Task DeleteOrderItemAsync(int orderItemId);
}
