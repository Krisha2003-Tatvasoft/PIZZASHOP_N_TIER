using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class OrderService : IOrderService
{

    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<(List<OrderTable>, int totalOrder)> GetOrderTable(int page, int pageSize, string search, 
    string SortColumn, string SortOrder,string status,DateTime? fromDate,DateTime? toDate)
    {
        var orderList = await _orderRepository.OrderTable(search, SortColumn, SortOrder,status,fromDate,toDate);

        if (orderList == null)
        {
            Console.WriteLine("orderList is null ");
        }

        int totalOrder =await orderList.CountAsync();

        var orders = await orderList
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(o => new OrderTable
        {
            Orderid = o.Orderid,
            Orderdate = o.Orderdate,
            Customername = o.Customer.Customername,
            orderstatus = (Entity.Models.Enums.orderstatus)o.status,
            paymentmode = (Entity.Models.Enums.paymentmode)o.Paymentmode,
            Rating = (decimal)o.Rating,
            Totalamount = o.Totalamount
        })
        .ToListAsync();

        return (orders, totalOrder);
    }
}
