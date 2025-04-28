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

    private readonly IOrderTaxRepository _orderTaxRepository;

    public OrderAppMenuService(IItemRepository itemRepository, IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository, IOrderItemModifierRepository orderItemModifierRepository,
    ITaxesRepository taxesRepository, IOrderTaxRepository orderTaxRepository)
    {
        _itemRepository = itemRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _orderItemModifierRepository = orderItemModifierRepository;
        _taxRepository = taxesRepository;
        _orderTaxRepository = orderTaxRepository;
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
                Orderitemid = i.Ordereditemid,
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

            var ordertaxes = await _orderTaxRepository.GetTaxesByOrderIdAsync(order.Orderid);
            List<OrderTax> orderTaxes = ordertaxes.Select(t => new OrderTax
            {
                Orderid = t.Orderid,
                Taxid = t.Taxid,
                Taxvalue = t.Taxvalue
            }).ToList();



            Bill model = new Bill
            {
                Orderid = order.Orderid,
                Tablenames = order.Ordertables.Select(t => t.Table.Tablename).ToList(),
                Sectionname = order.Ordertables.FirstOrDefault()?.Table.Section.Sectionname,
                Items = items,
                Taxes = taxes,
                OrderTax = orderTaxes
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
            var existingItemIds = existingOrderItems.Select(x => x.Ordereditemid).ToList();

            var incomingItemIds = model.Items.Where(x => x.Orderitemid != 0).Select(x => x.Orderitemid).ToList();

            var itemsToDelete = existingOrderItems.Where(x => !incomingItemIds.Contains(x.Ordereditemid)).ToList();
            foreach (var itemToDelete in itemsToDelete)
            {
                // Delete the modifiers for the item
                await _orderItemModifierRepository.DeleteModifiersByOrderItemIdAsync(itemToDelete.Ordereditemid);

                // Delete the order item itself
                await _orderItemRepository.DeleteOrderItemAsync(itemToDelete.Ordereditemid);
            }


            // Loop through the items in the incoming order
            foreach (var item in model.Items)
            {

                if (item.Orderitemid == 0)
                {
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
                    var existingItem = existingOrderItems.FirstOrDefault(x => x.Ordereditemid == item.Orderitemid);
                    if (existingItem != null)
                    {
                        existingItem.Quantity = item.Quantity;
                        await _orderItemRepository.UpdateOrderItemAsync(existingItem);
                    }
                }
            }


            // tax mapping....
            var existingTaxes = await _orderTaxRepository.GetTaxesByOrderIdAsync(model.Orderid);

            // Update or Add
            foreach (var newTax in model.OrderTax)
            {
                var existingTax = existingTaxes.FirstOrDefault(x => x.Taxid == newTax.Taxid);
                if (existingTax != null)
                {
                    // Update existing tax amount
                    existingTax.Taxvalue = newTax.Taxvalue;
                    await _orderTaxRepository.UpdateAsync(existingTax);
                }
                else
                {
                    // New tax, add it
                    var taxToAdd = new Ordertaxmapping
                    {
                        Orderid = model.Orderid,
                        Taxid = newTax.Taxid,
                        Taxvalue = newTax.Taxvalue
                    };
                    await _orderTaxRepository.AddAsync(taxToAdd);
                }
            }

            // Delete taxes that are no longer selected
            foreach (var existingTax in existingTaxes)
            {
                if (!model.OrderTax.Any(t => t.Taxid == existingTax.Taxid))
                {
                    await _orderTaxRepository.DeleteAsync(existingTax);
                }
            }

            return (true, "Order Saved Sucessfully.");
        }
        else
        {
            return (false, "No Order.");
        }

    }

    public async Task<string?> GetOrderComment(int id)
    {
        Order order = await _orderRepository.OrderDetailsByIdAsync(id);
        return order?.Orderwisecomment;
    }

     public async Task<bool> AddOrderComment(string comment , int orderid)
    {
        Order order = await _orderRepository.OrderDetailsByIdAsync(orderid);
        if (order == null)
        {
            return false;
        }
        else
        {
            order.Orderwisecomment = comment;
            await _orderRepository.UpdateOrder(order);
            return true;
        }
        
    }
}



