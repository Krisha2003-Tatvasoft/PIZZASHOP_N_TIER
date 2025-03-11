using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IItemService 
{
  List<ItemTable> GetItemTable(int id);

  Task<AddItem> Additem();
}
