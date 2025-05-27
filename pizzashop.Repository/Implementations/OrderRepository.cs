using System.Collections.Concurrent;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
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
            _ => orderList.OrderByDescending(o => o.Orderdate),// Default sort (if none provided)
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
        // var order = await _context.Orders
        //     .Include(o => o.Ordereditems)
        //     .FirstOrDefaultAsync(o => o.Orderid == model.OrderId);

        // if (order == null) return;
        // var allOrderditem = order.Ordereditems.ToList();
        // bool NotAnyReady = allOrderditem.All(i => i.ReadyQuantity == 0);

        // foreach (var ItemQuantity in model.Items)
        // {
        //     var orderedItem = order.Ordereditems.FirstOrDefault(i => i.Ordereditemid == ItemQuantity.orderitemid);

        //     if (orderedItem != null)
        //     {
        //         if (model.NewStatus == "Ready")
        //         {
        //             orderedItem.ReadyQuantity += ItemQuantity.Quantity;
        //         }
        //         else if (model.NewStatus == "Inprogress")
        //         {
        //             orderedItem.ReadyQuantity -= ItemQuantity.Quantity;
        //             if (orderedItem.ReadyQuantity < 0) orderedItem.ReadyQuantity = 0;
        //         }
        //     }
        // }

        // if (model.NewStatus == "Ready" && NotAnyReady)
        // {
        //     order.ServedTime = DateTime.Now;
        //     _context.Orders.Update(order);
        //     await _context.SaveChangesAsync();
        // }

        // await _context.SaveChangesAsync();

        var itemsJson = JsonConvert.SerializeObject(model.Items);

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var parameters = new
        {
            p_order_id = model.OrderId,
            p_new_status = model.NewStatus,
            p_items = itemsJson
        };

        if (connection is Npgsql.NpgsqlConnection npgsqlConnection)
        {
            npgsqlConnection.Notice += (sender, e) =>
            {
                Console.WriteLine("PG NOTICE: " + e.Notice.MessageText);
            };
        }



        await connection.ExecuteAsync(
            "CALL update_item_status(@p_order_id, @p_new_status, @p_items::jsonb);",
            parameters);



        Console.WriteLine(parameters.p_order_id);
        Console.WriteLine(parameters.p_new_status);
        Console.WriteLine(parameters.p_items);

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
        var customerGrowth = await _context.Customers
      .Where(c => c.Createdat.HasValue &&
                  (!fromDate.HasValue || c.Createdat.Value >= fromDate.Value) &&
                  (!toDate.HasValue || c.Createdat.Value <= toDate.Value))
      .GroupBy(c => c.Createdat)
      .Select(g => new ChartPoint
      {
          Date = (DateTime)g.Key,
          TotalCustomers = g.Count()
      })
      .OrderBy(g => g.Date)
      .ToListAsync();


        // Calculate the average waiting time for all waiting tokens within the filtered time range
        var avgWaitingTimeInMinutes = await _context.Orders
            .Where(w => w.Createdat.HasValue && w.ServedTime.HasValue &&
                        (!fromDate.HasValue || w.Createdat >= fromDate.Value) &&
                        (!toDate.HasValue || w.Createdat <= toDate.Value))
            .Select(w => (int?)(w.ServedTime.Value - w.Createdat.Value).TotalMinutes) // Calculate the difference in minutes
            .AverageAsync();

        string avgWaitingTimeFormatted;
        if (avgWaitingTimeInMinutes.HasValue)
        {
            avgWaitingTimeFormatted = $"{Math.Round(avgWaitingTimeInMinutes.Value, 2)} mins.";
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

    public async Task<List<TicketResult>> GetTicketResultFromSP(int categoryId, int status)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<TicketResult>(
            "SELECT * FROM get_tickets(@p_categoryid, @p_status);",
            new { p_categoryid = categoryId, p_status = status }
        );

        return result.ToList();
    }

    public async Task<List<TicketDetailResult>> GetTicketDetailsFromSP(int orderId, int status, int categoryId)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<TicketDetailResult>(
              "SELECT * FROM get_ticket_by_orderid(@p_orderid, @p_status,@p_categoryId);",
           new { p_orderid = orderId, p_status = status, p_categoryid = categoryId }
        );

        return result.ToList();
    }

    public async Task<List<OrderDetailsFlat>> GetOrderDetailsSp(int orderId)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<OrderDetailsFlat>(
              "SELECT * FROM get_order_details_flat(@order_id);",
           new { order_id = orderId }
        );

        return result.ToList();
    }

    public async Task<List<TaxTable>> GetAllTaxEnabledSp()
    {

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<TaxTable>(
              "SELECT * FROM get_enabled_taxes();"
        );

        return result.ToList();
    }


    public async Task<(bool Success, string Message)> SaveOrder_SP(int orderId, List<OrderItem> items, List<OrderTax> taxes)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("save_order_incremental", connection)
        {
            CommandType = CommandType.StoredProcedure
        };


        command.Parameters.AddWithValue("in_orderid", orderId);
        command.Parameters.AddWithValue("in_items", NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(items));
        command.Parameters.AddWithValue("in_taxes", NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(taxes));

        var successParam = new NpgsqlParameter("out_success", DbType.Boolean)
        {
            Direction = ParameterDirection.Output
        };
        var messageParam = new NpgsqlParameter("out_message", DbType.String)
        {
            Size = 500,
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);
        command.Parameters.Add(messageParam);

        await command.ExecuteNonQueryAsync();

        bool success = (bool)successParam.Value;
        string message = messageParam.Value?.ToString();

        return (success, message);
    }

    public async Task<(bool Success, string Message)> CancelOrder_SP(int orderId)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("cancel_order", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("in_orderid", orderId);

        var successParam = new NpgsqlParameter("out_success", DbType.Boolean)
        {
            Direction = ParameterDirection.Output
        };
        var messageParam = new NpgsqlParameter("out_message", DbType.String)
        {
            Size = 500,
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);
        command.Parameters.Add(messageParam);

        await command.ExecuteNonQueryAsync();

        bool success = (bool)successParam.Value;
        string message = messageParam.Value?.ToString();

        return (success, message);
    }


    public async Task<(bool Success, string Message)> CompleteOrder_SP(int orderId)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("complete_order", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("in_orderid", orderId);

        var successParam = new NpgsqlParameter("out_success", DbType.Boolean)
        {
            Direction = ParameterDirection.Output
        };
        var messageParam = new NpgsqlParameter("out_message", DbType.String)
        {
            Size = 500,
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);
        command.Parameters.Add(messageParam);

        await command.ExecuteNonQueryAsync();

        bool success = (bool)successParam.Value;
        string message = messageParam.Value?.ToString();

        return (success, message);
    }

    public async Task<(bool Success, string Message)> AddReview_SP(Review model)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("add_review", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("in_orderid", model.Orderid);
        command.Parameters.AddWithValue("in_foodrating", model.Foodrating);
        command.Parameters.AddWithValue("in_servicerating", model.Servicerating);
        command.Parameters.AddWithValue("in_ambiencerating", model.Ambiencerating);
        command.Parameters.AddWithValue("in_comments", (object?)model.Comments ?? DBNull.Value);

        var successParam = new NpgsqlParameter("out_success", DbType.Boolean)
        {
            Direction = ParameterDirection.Output
        };
        var messageParam = new NpgsqlParameter("out_message", DbType.String)
        {
            Size = 500,
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);
        command.Parameters.Add(messageParam);

        await command.ExecuteNonQueryAsync();

        bool success = (bool)successParam.Value;
        string message = messageParam.Value?.ToString();

        return (success, message);
    }

    public async Task<CustomerDetail?> GetCustomerDetail_SP(int orderId)
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        string sql = "SELECT * FROM get_customer_detail_by_orderid(@in_orderid);";

        var result = await connection.QueryFirstOrDefaultAsync<CustomerDetail>(
            sql,
            new { in_orderid = orderId }
        );

        return result;
    }

    public async Task<(bool success, string message)> EditCustomerDetail_SP(CustomerDetail model)
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var command = new NpgsqlCommand("edit_customer_detail", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("in_orderid", model.Orderid);
        command.Parameters.AddWithValue("in_customername", model.Customername ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("in_phoneno", model.Phoneno ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("in_email", model.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("in_oldemail", model.oldemail ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("in_noofperson", model.Noofperson ?? 0);

        var successParam = new NpgsqlParameter("out_success", DbType.Boolean) { Direction = ParameterDirection.Output };
        var messageParam = new NpgsqlParameter("out_message", DbType.String) { Size = 500, Direction = ParameterDirection.Output };

        command.Parameters.Add(successParam);
        command.Parameters.Add(messageParam);

        await command.ExecuteNonQueryAsync();

        return ((bool)successParam.Value, messageParam.Value?.ToString());
    }

    public async Task<Order> GetOrderBYId_SQL(int ordrid)
    {
        return await _context.Orders.FromSqlRaw("SELECT * FROM Orders where orderid = {0}", ordrid).FirstOrDefaultAsync();
    }

    public async Task<bool> AddOrderComment_SP(int orderId, string comment)
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var command = new NpgsqlCommand("sp_add_order_comment", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("p_orderid", orderId);
        command.Parameters.AddWithValue("p_comment", comment);

        var successParam = new NpgsqlParameter("p_success", DbType.Boolean)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);


        await command.ExecuteNonQueryAsync();

        bool success = successParam.Value != DBNull.Value && (bool)successParam.Value;
        return success;
    }




}
