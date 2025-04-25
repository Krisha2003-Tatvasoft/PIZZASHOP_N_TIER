using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IOrderItemModifierRepository
{
    Task<List<Ordereditemmodifer>> GetModifiersByOrderItemIdAsync(int orderItemId);

    Task AddOrderItemModifierAsync(Ordereditemmodifer orderItemModifier);

    Task DeleteModifiersByOrderItemIdAsync(int orderItemId);
}
