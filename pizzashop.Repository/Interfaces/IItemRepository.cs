using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IItemRepository
{
   Task<List<Item>> GetItemByCat(int id,string search);

   Task<bool> DeleteByCat(int id);

   Task AddNewItemAsync(Item item);

   Task<Item> ItemByIdAsync(int id);

   Task UpdateItem(Item item);
   
   Task DeleteItem(Item item);

   Task DeleteSelected(List<int> SelectedIds);
}
