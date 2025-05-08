using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PizzashopContext _context;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrderRepository(PizzashopContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
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
            var orderedItem = order.Ordereditems.FirstOrDefault(i => i.Ordereditemid == ItemQuantity.orderitemid);

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

    public async Task UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<DashboardViewModel> GetDashboardData(DateTime? fromDate, DateTime? toDate)
    {
        toDate = toDate.HasValue ? toDate.Value.AddDays(1).AddTicks(-1) : toDate;
        
        // Filter orders based on date range
        var ordersQuery = _context.Orders
            .Include(o => o.Ordereditems).ThenInclude(oi => oi.Item)
            .Include(o => o.Customer)
            .AsQueryable();

        if (fromDate.HasValue)
            ordersQuery = ordersQuery.Where(o => o.Orderdate >= fromDate.Value);
        if (toDate.HasValue)
            ordersQuery = ordersQuery.Where(o => o.Orderdate <= toDate.Value);

        // Execute the orders query
        var orders = await ordersQuery
            .Select(o => new
            {
                o.Totalamount,
                o.Orderdate,
                o.Customer.Customerid,
                Items = o.Ordereditems.Select(oi => new
                {
                    oi.Itemid,
                    oi.Item.Itemname,
                    oi.Item.Itemimg
                }).ToList()
            })
            .ToListAsync();

        // Calculate total sales, total orders, and average order value
        var totalSales = orders.Sum(o => o.Totalamount);
        var totalOrders = orders.Count;
        var avgOrder = totalOrders > 0 ? Math.Round(totalSales / totalOrders, 2) : 0;

        // Get the count of waiting tokens
        var waitingCount = await _context.Waitingtokens
        .Where(w => w.Createdat.HasValue && w.Createdat.Value.Date == DateTime.Today)
         .CountAsync();

        // Get the count of new customers
        var newCustomers = await _context.Customers
            .Where(c => (!fromDate.HasValue || c.Createdat >= fromDate.Value) &&
                        (!toDate.HasValue || c.Createdat <= toDate.Value))
            .CountAsync();

        // Flatten the items from all orders
        var flatItems = orders.SelectMany(o => o.Items).ToList();

        // Get top-selling items
        var topSelling = flatItems
            .GroupBy(i => i.Itemid)
            .Select(g => new ItemStats
            {
                Itemid = g.Key,
                Itemname = g.First().Itemname,
                Itemimg = g.First().Itemimg != null
            ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/{g.First().Itemimg}"
            : null,
                TotalOrderItem = g.Count()
            })
            .OrderByDescending(x => x.TotalOrderItem)
            .Take(5)
            .ToList();

        // Get least-selling items
        var leastSelling = flatItems
            .GroupBy(i => i.Itemid)
            .Select(g => new ItemStats
            {
                Itemid = g.Key,
                Itemname = g.First().Itemname,
                Itemimg = g.First().Itemimg != null
            ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/{g.First().Itemimg}"
            : null,
                TotalOrderItem = g.Count()
            })
            .OrderBy(x => x.TotalOrderItem)
            .Take(5)
            .ToList();

        // Calculate revenue growth data
        var revenueData = orders
            .GroupBy(o => o.Orderdate)
            .Select(g => new ChartPoint
            {
                Date = (DateTime)g.Key,
                TotalRevenue = g.Sum(o => o.Totalamount)
            })
            .ToList();

        // Calculate customer growth data
        var customerGrowth = orders
            .GroupBy(o => o.Orderdate)
            .Select(g => new ChartPoint
            {
                Date = (DateTime)g.Key,
                TotalCustomers = g.Select(o => o.Customerid).Distinct().Count()
            })
            .ToList();


        // Calculate the average waiting time for all waiting tokens within the filtered time range
        var avgWaitingTimeInMinutes = await _context.Waitingtokens
            .Where(w => w.Createdat.HasValue && w.Modifiedat.HasValue &&
                        (!fromDate.HasValue || w.Createdat >= fromDate.Value) &&
                        (!toDate.HasValue || w.Createdat <= toDate.Value))
            .Select(w => (int?)(w.Modifiedat.Value - w.Createdat.Value).TotalMinutes) // Calculate the difference in minutes
            .AverageAsync();

        // Round the average waiting time to the nearest whole number
        var roundedAvgWaitingTimeInMinutes = avgWaitingTimeInMinutes.HasValue
            ? (int)Math.Round(avgWaitingTimeInMinutes.Value)
            : (int?)null;

        // Convert the average waiting time into hours and minutes
        string avgWaitingTimeFormatted;
        if (roundedAvgWaitingTimeInMinutes.HasValue)
        {
            var totalMinutes = roundedAvgWaitingTimeInMinutes.Value;

            var hours = totalMinutes / 60; // Calculate hours
            var minutes = totalMinutes % 60 + (totalMinutes % 1); // Calculate remaining minutes with decimals

            if (hours > 0)
            {
                avgWaitingTimeFormatted = minutes > 0
                    ? $"{hours} hour{(hours > 1 ? "s" : "")} {Math.Round((double)minutes, 2)} min{(minutes > 1 ? "s" : "")}"
                    : $"{hours} hour{(hours > 1 ? "s" : "")}";
            }
            else
            {
                avgWaitingTimeFormatted = $"{Math.Round((double)minutes, 2)} min{(minutes > 1 ? "s" : "")}";
            }
        }
        else
        {
            avgWaitingTimeFormatted = "N/A"; // Handle case where no data is available
        }

        // Return the dashboard data
        return new DashboardViewModel
        {
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            AvgOrder = avgOrder,
            AvgWaitingTime = avgWaitingTimeFormatted,
            WaitingCount = waitingCount,
            NewCustomers = newCustomers,
            TopSellingItems = topSelling,
            LeastSellingItems = leastSelling,
            RevenueGrowthData = revenueData,
            CustomerGrowthData = customerGrowth
        };
    }


}
