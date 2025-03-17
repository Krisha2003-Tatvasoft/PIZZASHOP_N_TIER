using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IItemService 
{
 Task<(List<ItemTable> , int totalitem)> GetItemTable(int id,int page, int pageSize, string search);

  Task<AddItem> Additem();

  Task<bool> AddItemPost(int loginId,AddItem viewmodel);

  Task<AddItem> EditItem(int id);

  Task<bool> EditItemPost(int loginid, AddItem viewmodel);

  Task<bool> DeleteItem(int id);

  Task<bool> DeleteSelectedItem(List<int> selectedIds);


}
