using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;


namespace pizzashop.Service.Implementations;

public class ItemService :IItemService
{
    private readonly IItemRepository _itemRepository;
     private readonly ICategoryRepository _categoryRepository;

     private readonly IUnitRepository _unitRpository;

      private readonly IModifiersGroupRepository _modifiersGropRepository ;
     

    public ItemService(IItemRepository itemRepository , ICategoryRepository categoryRepository
    ,IUnitRepository unitRepository,IModifiersGroupRepository modifiersGropRepository)
    {
        _itemRepository = itemRepository;
        _categoryRepository = categoryRepository;
        _unitRpository = unitRepository;
        _modifiersGropRepository = modifiersGropRepository;
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

    public async Task<AddItem> Additem()
    {

       AddItem model = new AddItem
       {
         Categories= await _categoryRepository.GetAllCatyAsync(),
         Units = await _unitRpository.GetAllUnitAsync(),
         MGList = await _modifiersGropRepository.GetAllMGAsync()
       };
       return  model;
    }
}
