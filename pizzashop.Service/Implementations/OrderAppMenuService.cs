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

    private readonly IOrderRepository _orderRepository;

    public OrderAppMenuService(IItemRepository itemRepository, IOrderRepository orderRepository)
    {
        _itemRepository = itemRepository;
        _orderRepository = orderRepository;
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

    public async Task<Bill?> OrderDetails(int id)
    {
        var order = await _orderRepository.OrderDetailsByIdAsync(id);

        if (order != null)
        {

            List<OrderItem> items = order?.Ordereditems?.Select(i => new OrderItem
            {
                Itemid = i.Itemid,
                Itemname = i.Item.Itemname,
                Rate = i.Item.Rate,
                Quantity = (short)i.Quantity,
                ReadyQuantity = i.ReadyQuantity !=null ? i.ReadyQuantity :0,
                Modifiers = i.Ordereditemmodifers.Select(m => new OrderModifier
                {
                    Modifierid = m.Modifiers.Modifierid,
                    Modifiername = m.Modifiers.Modifiername,
                    Quantity = (short)i.Quantity,
                    Rate = m.Modifiers.Rate
                }).ToList()
            }).ToList();

            Bill model = new Bill
            {
                Orderid = order.Orderid,
                Tablenames = order.Ordertables.Select(t => t.Table.Tablename).ToList(),
                Sectionname = order.Ordertables.FirstOrDefault()?.Table.Section.Sectionname,
                Items = items
            };
            return model;
        }

        return new Bill();
      

    }


}
