using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PizzashopContext _context;

    public OrderRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Order>> OrderTable(string search, string SortColumn, string SortOrder, string status, DateTime? fromDate, DateTime? toDate)
    {
        toDate = toDate.HasValue ? toDate.Value.AddDays(1).AddTicks(-1) : toDate;
        string lowerSearch = search.ToLower();
        var orderList = _context.Orders
        .Include(o => o.Customer)
        .Where(o => (string.IsNullOrEmpty(status) || o.status == int.Parse(status)) &&
            (!fromDate.HasValue || o.Orderdate >= fromDate) &&
            (!toDate.HasValue || o.Orderdate <= toDate))
        .Where(u => string.IsNullOrEmpty(lowerSearch) ||
                            u.Customer.Customername.ToLower().Contains(lowerSearch) ||
                            u.Orderid.ToString().ToLower().Contains(lowerSearch) ||
                            u.Orderdate.ToString().ToLower().Contains(lowerSearch) ||
                            u.Totalamount.ToString().ToLower().Contains(lowerSearch));

        orderList = SortColumn switch
        {
            "OrderId" => SortOrder == "desc" ? orderList.OrderByDescending(o => o.Orderid) : orderList.OrderBy(o => o.Orderid),
            "OrderDate" => SortOrder == "desc" ? orderList.OrderByDescending(o => o.Orderdate) : orderList.OrderBy(o => o.Orderdate),
            "CustomerName" => SortOrder == "desc" ? orderList.OrderByDescending(o => o.Customer.Customername) : orderList.OrderBy(o => o.Customer.Customername),
            "TotalAmount" => SortOrder == "desc" ? orderList.OrderByDescending(o => o.Totalamount) : orderList.OrderBy(o => o.Totalamount),
            // Add additional cases for other columns as needed
            _ => orderList.OrderBy(o => o.Orderid),// Default sort (if none provided)
        };
        return orderList;
    }

    public async Task<IQueryable<Order>> OrderExcelTable(string search, string status, DateTime? fromDate, DateTime? toDate)
    {
        toDate = toDate.HasValue ? toDate.Value.AddDays(1).AddTicks(-1) : toDate;
        string lowerSearch = search.ToLower();
        var orderList = _context.Orders
        .Include(o => o.Customer)
        .Where(o => (string.IsNullOrEmpty(status) || o.status == int.Parse(status)) &&
            (!fromDate.HasValue || o.Orderdate >= fromDate) &&
            (!toDate.HasValue || o.Orderdate <= toDate))
        .Where(u => string.IsNullOrEmpty(lowerSearch) ||
                            u.Customer.Customername.ToLower().Contains(lowerSearch) ||
                            u.Orderid.ToString().ToLower().Contains(lowerSearch) ||
                            u.Orderdate.ToString().ToLower().Contains(lowerSearch) ||
                            u.Totalamount.ToString().ToLower().Contains(lowerSearch));


        return orderList;

    }

    public async Task<Order> OrderDetailsByIdAsync(int id)
    {
        return await _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.Invoices)
        .Include(o => o.Ordertables)
          .ThenInclude(t => t.Table)
          .ThenInclude(s => s.Section)
        .Include(o => o.Ordereditems).ThenInclude(i => i.Item)
        .Include(o => o.Ordereditems).ThenInclude(i => i.Ordereditemmodifers).ThenInclude(m => m.Modifiers)
        .Include(o => o.Ordertaxmappings).ThenInclude(t => t.Tax)
        .FirstOrDefaultAsync(o => o.Orderid == id);
    }


    public async Task AddNewOrder(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Order>> InprogressOrders()
    {
        return await _context.Orders.Where(o => o.status == 0).ToListAsync();
    }

    public async Task UpdateItemStatusAsync(OrderItemStatus model)
    {
        var order = await _context.Orders
            .Include(o => o.Ordereditems)
            .FirstOrDefaultAsync(o => o.Orderid == model.OrderId);

        if (order == null) return;

        foreach (var ItemQuantity in model.Items)
        {
            var orderedItem = order.Ordereditems.FirstOrDefault(i => i.Itemid == ItemQuantity.ItemId);

            if (orderedItem != null)
            {
                if (model.NewStatus == "Ready")
                {
                    orderedItem.ReadyQuantity += ItemQuantity.Quantity;
                }
                else if (model.NewStatus == "Inprogress")
                {
                    orderedItem.ReadyQuantity -= ItemQuantity.Quantity;
                    if (orderedItem.ReadyQuantity < 0) orderedItem.ReadyQuantity = 0;
                }
            }
        }

        await _context.SaveChangesAsync();
    }





}
