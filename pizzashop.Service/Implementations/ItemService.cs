using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;


namespace pizzashop.Service.Implementations;

public class ItemService :IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public List<ItemTable> GetItemTable(int id)
    {
        var itemList = _itemRepository.GetItemByCat(id);

        var items= itemList.Select(i => new ItemTable
        {
            Itemid = i.Itemid,
            Itemname=i.Itemname,
            Rate=i.Rate,
            Quantity = i.Quantity,
            Unitname = i.Unit.Unitname
        }
        ).ToList();

      return items;
    }
}
