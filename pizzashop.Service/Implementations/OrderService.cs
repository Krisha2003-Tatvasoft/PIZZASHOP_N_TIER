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

        if (orderList == null)
        {
            return (new List<OrderTable>(), 0);
        }

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
            Rating = (decimal)(o.Rating ?? 0),
            Totalamount = o.Totalamount
        })
        .ToListAsync();

        return (orders, totalOrder);
    }


    public async Task<List<OrderTable>> GetExcelOrderTable(string search,
        string status, DateTime? fromDate, DateTime? toDate)
    {
        var orderList = await _orderRepository.OrderExcelTable(search, status, fromDate, toDate);
        if (orderList == null)
        {
            return new List<OrderTable>();
        }

        var orders = await orderList
        .Select(o => new OrderTable
        {
            Orderid = o.Orderid,
            Orderdate = o.Orderdate,
            Customername = o.Customer.Customername,
            orderstatus = (Entity.Models.Enums.orderstatus)o.status,
            paymentmode = (Entity.Models.Enums.paymentmode)o.Paymentmode,
            Rating = (decimal)(o.Rating ?? 0),
            Totalamount = o.Totalamount
        })
        .ToListAsync();

        return orders;
    }

    public async Task<OrderDetails> OrderDetails(int id)
    {
        // var order = await _orderRepository.OrderDetailsByIdAsync(id);

        // List<OrderItem> items = order.Ordereditems.Select(i => new OrderItem
        // {
        //     Itemid = i.Itemid,
        //     Itemname = i.Item.Itemname,
        //     Rate = i.Item.Rate,
        //     Quantity = (short)i.Quantity,
        //     TotalAmount = i.Item.Rate * i.Quantity,
        //     Modifiers = i.Ordereditemmodifers.Select(m => new OrderModifier
        //     {
        //         Modifierid = m.Modifiers.Modifierid,
        //         Modifiername = m.Modifiers.Modifiername,
        //         Quantity = (short)i.Quantity,
        //         Rate = m.Modifiers.Rate,
        //         TotalAmount = m.Modifiers.Rate * i.Quantity
        //     }).ToList()
        // }).ToList();



        // List<TaxTable> taxes = order.Ordertaxmappings.Select(t => new TaxTable
        // {
        //     Taxid = t.Taxid,
        //     Taxname = t.Tax.Taxname,
        //     Taxvalue = t.Taxvalue?.ToString() ?? string.Empty
        // }).ToList();

        // OrderDetails model = new OrderDetails
        // {
        //     Orderid = order.Orderid,
        //     PlacedOn = order.Createdat,
        //     Customername = order.Customer.Customername,
        //     Phoneno = order.Customer.Phoneno,
        //     Noofperson = order.Noofperson,
        //     orderstatus = (Entity.Models.Enums.orderstatus)order.status,
        //     Email = order.Customer.Email,
        //     Invoicenumber = order.Invoices.FirstOrDefault()?.Invoicenumber,
        //     Tablenames = order.Ordertables.Select(t => t.Table.Tablename).ToList(),
        //     Sectionname = order.Ordertables.Select(t => t.Table.Section.Sectionname).Distinct().ToList(),
        //     Items = items,
        //     Taxes = taxes,
        //     Totalamount = order.Totalamount,
        //     Subamount = order.Subamount,
        //     paymentmode = (Entity.Models.Enums.paymentmode)order.Paymentmode
        // };
        // return model;

            List<OrderDetailsFlat> flatList = await _orderRepository.GetOrderDetailsSp(id);

            if (!flatList.Any())
                return null;

            List<OrderItem> items = flatList
                    .Where(f => f.Ordereditemid > 0)
                    .GroupBy(f => f.Ordereditemid)
                    .Select(g =>
                    {
                        OrderDetailsFlat first = g.First();
                        return new OrderItem
                        {
                            Itemid = first.Itemid,
                            Itemname = first.Itemname,
                            Rate = first.Rate,
                            Quantity = first.Quantity,
                            TotalAmount = first.Rate * first.Quantity,
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
                                Rate = mod.Modifierrate!.Value,
                                TotalAmount = mod.Modifierrate!.Value * first.Quantity
                            };
                        }).ToList()
                        };
                    }).ToList();

        var taxes = flatList
        .Where(f => f.Taxid.HasValue)
        .GroupBy(f => f.Taxid.Value)
        .Select(g =>
        {
            var first = g.First();
            return new TaxTable
            {
                Taxid = first.Taxid.Value,
                Taxvalue = first.Taxvalue?.ToString() ?? string.Empty,
                Taxname = first.taxname ?? string.Empty
            };
        })
        .ToList();     

            OrderDetails model = new OrderDetails
            {
                Orderid = flatList.First().Orderid,
                PlacedOn = flatList.First().Createdat,
                Customername = flatList.First().Customername,
                Phoneno = flatList.First().Phoneno,
                Noofperson = flatList.First().Noofperson,
                orderstatus = (Entity.Models.Enums.orderstatus)flatList.First().orderstatus,
                Email = flatList.First().Email,
                Invoicenumber = flatList.First().Invoicenumber,
                Tablenames = flatList.Select(f => f.Tablename).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList(),
                Sectionname = flatList.Select(f => f.Sectionname).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList(),
                Items = items,
                Taxes = taxes,
                Totalamount = (decimal)flatList.First().Totalamount,
                Subamount = flatList.First().Subamount,
                paymentmode = (Entity.Models.Enums.paymentmode)flatList.First().paymentmode
            };
            return model;


    }











}
