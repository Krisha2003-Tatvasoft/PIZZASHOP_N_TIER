using System.Linq;
using DocumentFormat.OpenXml.EMMA;
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

    private readonly IOrderItemRepository _orderItemRepository;

    private readonly IOrderItemModifierRepository _orderItemModifierRepository;

    private readonly ITaxesRepository _taxRepository;

    public OrderAppMenuService(IItemRepository itemRepository, IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository, IOrderItemModifierRepository orderItemModifierRepository, ITaxesRepository taxesRepository)
    {
        _itemRepository = itemRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _orderItemModifierRepository = orderItemModifierRepository;
        _taxRepository = taxesRepository;
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

            List<OrderItem>? items = order?.Ordereditems?.Select(i => new OrderItem
            {
                Itemid = i.Itemid,
                Itemname = i.Item.Itemname,
                Rate = i.Item.Rate,
                Quantity = (short)i.Quantity,
                ReadyQuantity = i.ReadyQuantity != null ? i.ReadyQuantity : 0,
                Isdefaulttax = i.Item.Isdefaulttax,
                Taxpercentage = i.Item.Taxpercentage,
                Modifiers = i.Ordereditemmodifers.Select(m => new OrderModifier
                {
                    Modifierid = m.Modifiers.Modifierid,
                    Modifiername = m.Modifiers.Modifiername,
                    Quantity = (short)i.Quantity,
                    Rate = m.Modifiers.Rate
                }).ToList()
            }).ToList();

            var taxlist = await _taxRepository.GetAllTaxEnabled();
            List<TaxTable> taxes = taxlist.Select(t => new TaxTable
            {
                Taxid = t.Taxid,
                Taxname = t.Taxname,
                taxtype = (Enums.taxtype)t.Taxtype,
                Taxvalue = t.Taxvalue
            }).ToList();

            Bill model = new Bill
            {
                Orderid = order.Orderid,
                Tablenames = order.Ordertables.Select(t => t.Table.Tablename).ToList(),
                Sectionname = order.Ordertables.FirstOrDefault()?.Table.Section.Sectionname,
                Items = items,
                Taxes = taxes
            };



            return model;
        }

        return new Bill();
    }

    private void ToList()
    {
        throw new NotImplementedException();
    }


    public async Task<(bool success, string message)> SaveOrder(Bill model)
    {
        var order = await _orderRepository.OrderDetailsByIdAsync(model.Orderid);
        if (order != null)
        {
            order.status = 0;
            await _orderRepository.UpdateOrder(order);
            var existingOrderItems = await _orderItemRepository.GetOrderItemsByOrderIdAsync(model.Orderid);

            // Loop through the items in the incoming order
            foreach (var item in model.Items)
            {
                var existingOrderItem = existingOrderItems.FirstOrDefault(oi => oi.Itemid == item.Itemid);

                if (existingOrderItem != null)
                {
                    // Item exists, check if modifiers are the same
                    var existingModifiers = await _orderItemModifierRepository.GetModifiersByOrderItemIdAsync(existingOrderItem.Ordereditemid);

                    // If modifiers are different, treat it as a new entry
                    if (!AreModifiersSame(existingModifiers, item.Modifiers))
                    {
                        // Create a new OrderItem as a new item
                        var newOrderItem = new Ordereditem
                        {
                            Orderid = model.Orderid,
                            Itemid = item.Itemid,
                            Quantity = item.Quantity,
                            ReadyQuantity = 0 // or calculate based on your logic
                        };
                        var addedOrderItem = await _orderItemRepository.AddOrderItemAsync(newOrderItem);

                        // Add new modifiers for this new item
                        foreach (var modifier in item.Modifiers)
                        {
                            var orderItemModifier = new Ordereditemmodifer
                            {
                                Ordereditemid = addedOrderItem.Ordereditemid,
                                modifierid = modifier.Modifierid
                            };
                            await _orderItemModifierRepository.AddOrderItemModifierAsync(orderItemModifier);
                        }
                    }
                    else
                    {
                        // No changes in modifiers, just update the quantity
                        existingOrderItem.Quantity = item.Quantity;
                        await _orderItemRepository.UpdateOrderItemAsync(existingOrderItem);
                    }
                }
                else
                {
                    // Item does not exist, so add a new order item
                    var newOrderItem = new Ordereditem
                    {
                        Orderid = model.Orderid,
                        Itemid = item.Itemid,
                        Quantity = item.Quantity,
                        ReadyQuantity = 0
                    };
                    var addedOrderItem = await _orderItemRepository.AddOrderItemAsync(newOrderItem);

                    // Add modifiers for this new item
                    foreach (var modifier in item.Modifiers)
                    {
                        var orderItemModifier = new Ordereditemmodifer
                        {
                            Ordereditemid = addedOrderItem.Ordereditemid,
                            modifierid = modifier.Modifierid
                        };
                        await _orderItemModifierRepository.AddOrderItemModifierAsync(orderItemModifier);
                    }
                }
            }

            // Now handle items that are no longer in the incoming order (i.e., deleted items)
            var incomingItemIds = model.Items.Select(i => i.Itemid).ToList();
            var itemsToDelete = existingOrderItems.Where(oi => !incomingItemIds.Contains(oi.Itemid)).ToList();

            foreach (var itemToDelete in itemsToDelete)
            {
                // Delete the modifiers for the item
                await _orderItemModifierRepository.DeleteModifiersByOrderItemIdAsync(itemToDelete.Ordereditemid);

                // Delete the order item itself
                await _orderItemRepository.DeleteOrderItemAsync(itemToDelete.Ordereditemid);
            }
            return (true, "Order Saved Sucessfully.");
        }
        else
        {
            return (false, "No Order.");
        }

    }

    // Check if modifiers are the same
    public bool AreModifiersSame(List<Ordereditemmodifer> existingModifiers, List<OrderModifier> newModifiers)
    {
        if (existingModifiers.Count != newModifiers.Count)
            return false;

        // Check if every modifier is the same
        foreach (var newModifier in newModifiers)
        {
            var existingModifier = existingModifiers.FirstOrDefault(m => m.modifierid == newModifier.Modifierid);
            if (existingModifier == null)
                return false;
        }
        return true;
    }


}
