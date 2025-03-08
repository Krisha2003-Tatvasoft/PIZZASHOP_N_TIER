using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IItemRepository
{
    List<Item> GetItemByCat(int id);

   Task<bool> DeleteByCat(int id);
}
