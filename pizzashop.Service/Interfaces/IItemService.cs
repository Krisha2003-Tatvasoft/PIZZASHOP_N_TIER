using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IItemService 
{
  Task<List<ItemTable>> GetItemTable(int id);

  Task<AddItem> Additem();

  Task<bool> AddItemPost(int loginId,AddItem viewmodel);

  Task<AddItem> EditItem(int id);

  Task<bool> EditItemPost(int loginid, AddItem viewmodel);

  Task<bool> DeleteItem(int id);

  
  
}
