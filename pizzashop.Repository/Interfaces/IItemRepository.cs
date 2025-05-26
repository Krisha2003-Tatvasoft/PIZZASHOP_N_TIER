using pizzashop.Entity.Models;
using IMGMviewmodel = pizzashop.Entity.ViewModels.IMGMviewmodel;

namespace pizzashop.Repository.Interfaces;

public interface IItemRepository
{
    Task<List<Item>> GetItemByCat(int id, string search);

    Task<bool> DeleteByCat(int id);

    Task AddNewItemAsync(Item item);

    Task<Item> ItemByIdAsync(int id);

    Task UpdateItem(Item item);

    Task DeleteItem(Item item);

    Task DeleteSelected(List<int> SelectedIds);

    Task<bool> ItemExistAsync(string Itemname);

    Task<bool> ItemNameExistAtEditAsync(string Itemname, int id);

    Task<List<Item>> GetMenuItem(string search);

    Task<Item> ItemWithModifier(int id);

    Task<List<Item>> GetMenuItemSP(string search, int categoryId);

    Task<List<IMGMviewmodel>> ModifiersByIdSP(int itemId);
    
}
