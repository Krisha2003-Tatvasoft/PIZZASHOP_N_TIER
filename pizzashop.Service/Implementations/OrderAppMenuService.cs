using System.Linq;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Service.Implementations;

public class OrderAppMenuService : IOrderAppMenuService
{
    private readonly IItemRepository _itemRepository;

    private readonly IOrderRepository _orderRepository;

    private readonly IOrderItemRepository _orderItemRepository;

    private readonly IOrderItemModifierRepository _orderItemModifierRepository;

    private readonly ITaxesRepository _taxRepository;

    private readonly IOrderTaxRepository _orderTaxRepository;

    private readonly ITableRepository _tableRepository;

    private readonly ICustomerRepository _customerRepository;

    private readonly ICustomerReviewRepository _customerReviewRepository;

    public OrderAppMenuService(IItemRepository itemRepository, IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository, IOrderItemModifierRepository orderItemModifierRepository,
    ITaxesRepository taxesRepository, IOrderTaxRepository orderTaxRepository, ITableRepository tableRepository,
    ICustomerRepository customerRepository, ICustomerReviewRepository customerReviewRepository)
    {
        _itemRepository = itemRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _orderItemModifierRepository = orderItemModifierRepository;
        _taxRepository = taxesRepository;
        _orderTaxRepository = orderTaxRepository;
        _tableRepository = tableRepository;
        _customerRepository = customerRepository;
        _customerReviewRepository = customerReviewRepository;
    }

    public async Task<List<IMGMviewmodel>> ModifiersById(int id)
    {

        // var item = await _itemRepository.ItemWithModifier(id);
        // List<IMGMviewmodel>? itemWithModifier = item?.Itemmodifiergroupmaps?.Select(im => new IMGMviewmodel
        // {
        //     Itemid = item.Itemid,
        //     Itemname = item.Itemname,
        //     Itemmodifiergroupid = im.Itemmodifiergroupid,
        //     Modifiergroupname = im.Modifiergroup.Modifiergroupname,
        //     Modifiergroupid = im.Modifiergroupid,
        //     Minselectionrequired = im.Minselectionrequired,
        //     Maxselectionallowed = im.Maxselectionallowed,
        //     itemtype = item.itemtype,
        //     modifiers = im.Modifiergroup?.ModifierGroupModifier?.Select(mgm => new Modifier
        //     {
        //         Modifierid = mgm.ModifierId,
        //         Modifiername = mgm.Modifier?.Modifiername,
        //         Rate = (decimal)(mgm.Modifier?.Rate),
        //     }).ToList()

        // }).ToList();

        // return itemWithModifier;

        return await _itemRepository.ModifiersByIdSP(id);

    }

    public async Task<Bill?> OrderDetails(int id)
    {
        // var order = await _orderRepository.OrderDetailsByIdAsync(id);

        // if (order != null)
        // {

        //     List<OrderItem>? items = order?.Ordereditems?.Select(i => new OrderItem
        //     {
        //         Orderitemid = i.Ordereditemid,
        //         Itemid = i.Itemid,
        //         Itemname = i.Item.Itemname,
        //         Rate = i.Item.Rate,
        //         Quantity = (short)i.Quantity,
        //         ReadyQuantity = i.ReadyQuantity != null ? i.ReadyQuantity : 0,
        //         Isdefaulttax = i.Item.Isdefaulttax,
        //         Taxpercentage = i.Item.Taxpercentage,
        //         Itemwisecomment = i.Itemwisecomment,
        //         Modifiers = i.Ordereditemmodifers.Select(m => new OrderModifier
        //         {
        //             Modifierid = m.Modifiers.Modifierid,
        //             Modifiername = m.Modifiers.Modifiername,
        //             Quantity = (short)i.Quantity,
        //             Rate = m.Modifiers.Rate
        //         }).ToList()
        //     }).ToList();

        //     var taxlist = await _taxRepository.GetAllTaxEnabled();
        //     List<TaxTable> taxes = taxlist.Select(t => new TaxTable
        //     {
        //         Taxid = t.Taxid,
        //         Taxname = t.Taxname,
        //         taxtype = (Enums.taxtype)t.Taxtype,
        //         Taxvalue = t.Taxvalue
        //     }).ToList();

        //     var ordertaxes = await _orderTaxRepository.GetTaxesByOrderIdAsync(order.Orderid);
        //     List<OrderTax> orderTaxes = ordertaxes.Select(t => new OrderTax
        //     {
        //         Orderid = t.Orderid,
        //         Taxid = t.Taxid,
        //         Taxvalue = t.Taxvalue
        //     }).ToList();


        //     Bill model = new Bill
        //     {
        //         Orderid = order.Orderid,
        //         Tablenames = order.Ordertables.Select(t => t.Table.Tablename).ToList(),
        //         Sectionname = order.Ordertables.Select(t => t.Table.Section.Sectionname).Distinct().ToList(),
        //         Items = items,
        //         Taxes = taxes,
        //         OrderTax = orderTaxes
        //     };

        //     return model;
        // }

        // return new Bill();
        if (id != 0)
        {
            List<OrderDetailsFlat> flatList = await _orderRepository.GetOrderDetailsSp(id);
            if (!flatList.Any())
                return null;

            List<TaxTable> taxList = await _orderRepository.GetAllTaxEnabledSp();

            List<OrderItem> items = flatList
                .Where(f => f.Ordereditemid > 0)
                .GroupBy(f => f.Ordereditemid)
                .Select(g =>
                {
                    OrderDetailsFlat first = g.First();
                    return new OrderItem
                    {
                        Orderitemid = first.Ordereditemid,
                        Itemid = first.Itemid,
                        Itemname = first.Itemname,
                        Rate = first.Rate,
                        Quantity = first.Quantity,
                        ReadyQuantity = first.Readyquantity,
                        Isdefaulttax = first.Isdefaulttax,
                        Taxpercentage = first.Taxpercentage,
                        Itemwisecomment = first.Itemwisecomment,
                        Modifiers = g
                      .Where(x => x.Modifierid != null)
                      .GroupBy(m => m.Modifierid)
                      .Select(mg =>
                    {
                        OrderDetailsFlat mod = mg.First();
                        return new OrderModifier
                        {
                            Modifierid = mod.Modifierid!.Value,
                            Modifiername = mod.Modifiername!,
                            Quantity = first.Quantity,
                            Rate = mod.Modifierrate!.Value
                        };
                    }).ToList()
                    };
                }).ToList();

            Bill bill = new Bill
            {
                Orderid = flatList.First().Orderid,
                Tablenames = flatList.Select(f => f.Tablename).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList(),
                Sectionname = flatList.Select(f => f.Sectionname).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList(),
                Items = items,
                Taxes = taxList,
                OrderTax = flatList.Where(f => f.Taxid.HasValue).Select(f => new OrderTax
                {
                    Orderid = f.Orderid,
                    Taxid = f.Taxid.Value,
                    Taxvalue = f.Taxvalue
                }).Distinct().ToList()
            };

            return bill;
        }
        else
        {
             return new Bill();
        }
    }

    private void ToList()
    {
        throw new NotImplementedException();
    }


    public async Task<(bool success, string message)> SaveOrder(Bill model)
    {
        // Order order = await _orderRepository.OrderDetailsByIdAsync(model.Orderid);
        // if (order != null)
        // {
        //     decimal subtotal = model.Items.Sum(x => x.Rate * x.Quantity + x.Modifiers.Sum(m => m.Rate * x.Quantity));
        //     decimal? tax = model.OrderTax.Sum(x => x.Taxvalue);
        //     order.status = 0;
        //     order.Subamount = subtotal;
        //     order.Totaltax = tax;
        //     order.Totalamount = (decimal)(subtotal + tax);
        //     await _orderRepository.UpdateOrder(order);

        //     List<Ordereditem> existingOrderItems = await _orderItemRepository.GetOrderItemsByOrderIdAsync(model.Orderid);
        //     List<int> existingItemIds = existingOrderItems.Select(x => x.Ordereditemid).ToList();

        //     List<int> incomingItemIds = model.Items.Where(x => x.Orderitemid != 0).Select(x => x.Orderitemid).ToList();

        //     List<Ordereditem> itemsToDelete = existingOrderItems.Where(x => !incomingItemIds.Contains(x.Ordereditemid)).ToList();
        //     foreach (Ordereditem? itemToDelete in itemsToDelete)
        //     {
        //         // Delete the modifiers for the item
        //         await _orderItemModifierRepository.DeleteModifiersByOrderItemIdAsync(itemToDelete.Ordereditemid);

        //         // Delete the order item itself
        //         await _orderItemRepository.DeleteOrderItemAsync(itemToDelete.Ordereditemid);
        //     }


        //     // Loop through the items in the incoming order
        //     foreach (OrderItem? item in model.Items)
        //     {

        //         if (item.Orderitemid == 0)
        //         {
        //             Ordereditem newOrderItem = new Ordereditem
        //             {
        //                 Orderid = model.Orderid,
        //                 Itemid = item.Itemid,
        //                 Quantity = item.Quantity,
        //                 ReadyQuantity = 0,
        //                 Itemwisecomment = item.Itemwisecomment
        //             };
        //             Ordereditem addedOrderItem = await _orderItemRepository.AddOrderItemAsync(newOrderItem);

        //             // Add new modifiers for this new item
        //             foreach (OrderModifier? modifier in item.Modifiers)
        //             {
        //                 Ordereditemmodifer orderItemModifier = new Ordereditemmodifer
        //                 {
        //                     Ordereditemid = addedOrderItem.Ordereditemid,
        //                     modifierid = modifier.Modifierid
        //                 };
        //                 await _orderItemModifierRepository.AddOrderItemModifierAsync(orderItemModifier);
        //             }
        //         }
        //         else
        //         {
        //             Ordereditem? existingItem = existingOrderItems.FirstOrDefault(x => x.Ordereditemid == item.Orderitemid);
        //             if (existingItem != null)
        //             {
        //                 existingItem.Quantity = item.Quantity;
        //                 existingItem.Itemwisecomment = item.Itemwisecomment;
        //                 await _orderItemRepository.UpdateOrderItemAsync(existingItem);
        //             }
        //         }
        //     }


        //     // tax mapping....
        //     List<Ordertaxmapping> existingTaxes = await _orderTaxRepository.GetTaxesByOrderIdAsync(model.Orderid);

        //     // Update or Add
        //     foreach (OrderTax? newTax in model.OrderTax)
        //     {
        //         Ordertaxmapping? existingTax = existingTaxes.FirstOrDefault(x => x.Taxid == newTax.Taxid);
        //         if (existingTax != null)
        //         {
        //             // Update existing tax amount
        //             existingTax.Taxvalue = newTax.Taxvalue;
        //             await _orderTaxRepository.UpdateAsync(existingTax);
        //         }
        //         else
        //         {
        //             // New tax, add it
        //             Ordertaxmapping taxToAdd = new Ordertaxmapping
        //             {
        //                 Orderid = model.Orderid,
        //                 Taxid = newTax.Taxid,
        //                 Taxvalue = newTax.Taxvalue
        //             };
        //             await _orderTaxRepository.AddAsync(taxToAdd);
        //         }
        //     }

        //     // Delete taxes that are no longer selected
        //     foreach (Ordertaxmapping existingTax in existingTaxes)
        //     {
        //         if (!model.OrderTax.Any(t => t.Taxid == existingTax.Taxid))
        //         {
        //             await _orderTaxRepository.DeleteAsync(existingTax);
        //         }
        //     }

        //     return (true, "Order Saved Sucessfully.");
        // }
        // else
        // {
        //     return (false, "No Order.");
        // }

        return await _orderRepository.SaveOrder_SP(model.Orderid, model.Items, model.OrderTax); 
    }

    public async Task<string?> GetOrderComment(int id)
    {
        // Order order = await _orderRepository.OrderDetailsByIdAsync(id);
        Order order = await _orderRepository.GetOrderBYId_SQL(id);
        return order?.Orderwisecomment;
    }

    public async Task<bool> AddOrderComment(string comment, int orderid)
    {
        // Order order = await _orderRepository.OrderDetailsByIdAsync(orderid);
        // if (order == null)
        // {
        //     return false;
        // }
        // else
        // {
        //     order.Orderwisecomment = comment;
        //     await _orderRepository.UpdateOrder(order);
        //     return true;
        // }

        return await _orderRepository.AddOrderComment_SP(orderid, comment);

    }


    public async Task<(bool success, string message)> CancelOrder(int orderid)
    {
        // Order order = await _orderRepository.OrderDetailsByIdAsync(orderid);
        // if (order == null)
        // {
        //     return (false, "No Order.");
        // }
        // else
        // {
        //     bool hasReadyItems = order.Ordereditems.Any(item => item.ReadyQuantity > 0);
        //     if (hasReadyItems)
        //     {
        //         return (false, "Cannot cancel order because some items are already prepared.");
        //     }

        //     order.status = 4;
        //     await _orderRepository.UpdateOrder(order);

        //     List<Ordertable> tables = order.Ordertables.ToList();
        //     foreach (Ordertable? table in tables)
        //     {
        //         Entity.Models.Table tableByid = await _tableRepository.TableByIdAsync(table.Tableid);
        //         tableByid.tablestatus = 0;
        //         await _tableRepository.UpdateTable(tableByid);
        //     }

        //     return (true, "Order Cancelled sucessfully.");

        // }

        return await _orderRepository.CancelOrder_SP(orderid);

    }


    public async Task<(bool success, string message)> CompleteOrder(int orderid)
    {
        // Order order = await _orderRepository.OrderDetailsByIdAsync(orderid);
        // if (order == null)
        // {
        //     return (false, "No Order.");
        // }
        // else
        // {
        //     bool hasInProgressItems = order.Ordereditems.Any(item => item.ReadyQuantity < item.Quantity);
        //     if (hasInProgressItems)
        //     {
        //         return (false, "Some items are still being prepared. Cannot complete the order!");
        //     }
        //     order.status = 3;
        //     await _orderRepository.UpdateOrder(order);

        //     List<Ordertable> tables = order.Ordertables.ToList();
        //     foreach (Ordertable? table in tables)
        //     {
        //         Entity.Models.Table tableByid = await _tableRepository.TableByIdAsync(table.Tableid);
        //         tableByid.tablestatus = 0;
        //         await _tableRepository.UpdateTable(tableByid);
        //     }
        //     return (true, "Order Completed Sucessfully.");

        // }

        return await _orderRepository.CompleteOrder_SP(orderid);
    }


    public async Task<CustomerDetail?> CustomerDetail(int orderid)
    {
        // Order order = await _orderRepository.OrderDetailsByIdAsync(orderid);
        // if (order != null)
        // {
        //     CustomerDetail Customer = new CustomerDetail
        //     {
        //         customerId = order.Customerid,
        //         Email = order.Customer?.Email,
        //         Customername = order.Customer?.Customername,
        //         Phoneno = order.Customer?.Phoneno,
        //         Noofperson = order.Noofperson,
        //         Orderid = order.Orderid,
        //         oldemail = order.Customer?.Email
        //     };
        //     return Customer;
        // }
        // return null;
        return await _orderRepository.GetCustomerDetail_SP(orderid);
    }

    public async Task<(bool sucess, string message)> EditCustomerdetail(CustomerDetail model)
    {
        // Order order = await _orderRepository.OrderDetailsByIdAsync((int)model.Orderid);
        // if (order != null)
        // {
        //     Customer customer = await _customerRepository.GetCustomerByEmail(model.Email);
        //     if (customer != null && customer.Customerid != model.customerId)
        //     {
        //         return (false, "Email already In Used.");
        //     }
        //     else
        //     {
        //         int totalCapacity = 0;
        //         List<Ordertable> tables = order.Ordertables.ToList();
        //         foreach (Ordertable? table in tables)
        //         {
        //             Entity.Models.Table tableid = await _tableRepository.TableByIdAsync(table.Tableid);
        //             totalCapacity += (int)tableid.Capacity;
        //         }
        //         if (totalCapacity < model.Noofperson)
        //         {
        //             return (false, "No. of Person " + model.Noofperson + " Is more Then the Table Capacity " + totalCapacity + ".");
        //         }
        //         else
        //         {
        //             Customer oldcustomer = await _customerRepository.GetCustomerByEmail(model.oldemail);

        //             if (oldcustomer != null)
        //             {
        //                 oldcustomer.Customername = model.Customername;
        //                 oldcustomer.Phoneno = model.Phoneno;
        //                 oldcustomer.Email = model.Email;
        //                 await _customerRepository.UpdateCustomer(oldcustomer);
        //             }

        //             order.Noofperson = (short)model.Noofperson;
        //             await _orderRepository.UpdateOrder(order);

        //             return (true, "Customer Details Updated Sucessfully.");
        //         }

        //     }
        // }
        // else
        // {
        //     return (false, "no order");
        // }

        return await _orderRepository.EditCustomerDetail_SP(model);
    }

    public async Task<(bool sucess, string message)> ReviewPost(Review model)
    {
        // if (model != null)
        // {
        //     Customerreview customerreview = new Customerreview
        //     {
        //         Orderid = model.Orderid,
        //         Foodrating = model.Foodrating,
        //         Servicerating = model.Servicerating,
        //         Ambiencerating = model.Ambiencerating,
        //         Avgrating = (float?)((model.Foodrating + model.Servicerating + model.Ambiencerating) / 3),
        //         Comments = model.Comments,
        //         Createdat = DateTime.Now
        //     };
        //     await _customerReviewRepository.AddNewReview(customerreview);
        //     Order order = await _orderRepository.OrderDetailsByIdAsync((int)model.Orderid);
        //     if (order != null)
        //     {
        //         order.Rating = (decimal?)customerreview.Avgrating;
        //         await _orderRepository.UpdateOrder(order);
        //     }
        //     return (true, "Review Added Sucessfully.");
        // }
        // else
        // {
        //     return (false, "No Review");
        // }

        return await _orderRepository.AddReview_SP(model);
    }


    public async Task<bool> checkOrderStatus(int id)
    {
        Order order = await _orderRepository.OrderDetailsByIdAsync(id);
        if (order != null)
        {
            if (order.status == 0 || order.status == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}



