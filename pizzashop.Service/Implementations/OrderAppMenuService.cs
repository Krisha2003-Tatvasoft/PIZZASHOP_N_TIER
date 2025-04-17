using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class OrderAppMenuService : IOrderAppMenuService
{
    private readonly IItemRepository _itemRepository;

    public OrderAppMenuService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<List<IMGMviewmodel>> ModifiersById(int id)
    {
         
        var item = await _itemRepository.ItemWithModifier(id);
        List<IMGMviewmodel>? itemWithModifier = item?.Itemmodifiergroupmaps?.Select(im => new IMGMviewmodel
        {
            Itemid = item.Itemid,
            Itemname = item.Itemname,
            Itemmodifiergroupid = im.Itemmodifiergroupid,
            Modifiergroupname = im.Modifiergroup.Modifiergroupname,
            Modifiergroupid = im.Modifiergroupid,
            Minselectionrequired = im.Minselectionrequired,
            Maxselectionallowed = im.Maxselectionallowed,
            itemtype = item.itemtype,
            modifiers = im.Modifiergroup?.ModifierGroupModifier?.Select(mgm => new Modifier
            {
                Modifierid = mgm.ModifierId,
                Modifiername = mgm.Modifier?.Modifiername,
                Rate = (decimal)(mgm.Modifier?.Rate),
            }).ToList()

        }).ToList();

        return itemWithModifier;

    }

}
