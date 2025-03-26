using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
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
        Console.WriteLine("helooooo"+fromDate);

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




}
