using DocumentFormat.OpenXml.Spreadsheet;
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
    string SortColumn, string SortOrder, string status, DateTime? fromDate, DateTime? toDate)
    {
        var orderList = await _orderRepository.OrderTable(search, SortColumn, SortOrder, status, fromDate, toDate);

        int totalOrder = await orderList.CountAsync();

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


    public async Task<List<OrderTable>> GetExcelOrderTable(string search,
        string status, DateTime? fromDate, DateTime? toDate)
    {
        var orderList = await _orderRepository.OrderExcelTable(search, status, fromDate, toDate);

        var orders = await orderList
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

        return orders;
    }

    public async Task<OrderDetails> OrderDetails(int id)
    {
        var order = await _orderRepository.OrderDetailsByIdAsync(id);

        List<OrderItem> items = order.Ordereditems.Select(i => new OrderItem
        {
            Itemid = i.Itemid,
            Itemname = i.Item.Itemname,
            Rate = i.Item.Rate,
            Quantity = (short)i.Quantity,
            TotalAmount = i.Item.Rate * i.Quantity,
            Modifiers = i.Ordereditemmodifers.Select(m => new OrderModifier
            {
                Modifierid = m.Modifiers.Modifierid,
                Modifiername = m.Modifiers.Modifiername,
                Quantity = (short)i.Quantity,
                Rate = m.Modifiers.Rate,
                TotalAmount = m.Modifiers.Rate * i.Quantity 
            }).ToList()
        }).ToList();

        decimal subTotal = order.Ordereditems.Sum
            (oi => (oi.Item.Rate + oi.Ordereditemmodifers.Sum(mod => mod.Modifiers.Rate)) * oi.Quantity);

        decimal TotalAmount = subTotal + order.Ordertaxmappings.Sum(t => decimal.TryParse(t.Tax.Taxvalue, out var taxValue) ? taxValue : 0);


        List<TaxTable> taxes = order.Ordertaxmappings.Select(t => new TaxTable
        {
            Taxid = t.Taxid,
            Taxname = t.Tax.Taxname,
            Taxvalue = t.Tax.Taxvalue
        }).ToList();

       OrderDetails model = new OrderDetails{
           Orderid= order.Orderid,
           PlacedOn = order.Createdat,
           Customername = order.Customer.Customername,
           Phoneno = order.Customer.Phoneno,
           Noofperson = order.Noofperson,
           orderstatus = (Entity.Models.Enums.orderstatus)order.status,
           Email=order.Customer.Email,
           Invoicenumber = order.Invoices.FirstOrDefault()?.Invoicenumber,
           Tablenames = order.Ordertables.Select(t => t.Table.Tablename).ToList(),
           Sectionname = order.Ordertables.FirstOrDefault()?.Table.Section.Sectionname,
           Items=items,
           Taxes=taxes,
           Totalamount=TotalAmount,
           Subamount= subTotal,
           paymentmode = (Entity.Models.Enums.paymentmode)order.Paymentmode
       };
     return model;
        
    }

    


    






}
