using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<(List<CustomerTable>, int totalCustomer)> GetCustomerTable(int page, int pageSize, string search,
   string SortColumn, string SortOrder, DateTime? fromDate, DateTime? toDate)
    {
        var custList = await _customerRepository.CustomerTable(search, SortColumn, SortOrder, fromDate, toDate);

        int totalCustomer = await custList.CountAsync();

        var customers = await custList
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(o => new CustomerTable
        {
            Customerid = o.Customerid,
            Customername = o.Customername,
            Email = o.Email,
            Phoneno = o.Phoneno,
            Orderdate = o.Createdat,
            Totalorder = o.Totalorder
        })
        .ToListAsync();


        return (customers, totalCustomer);
    }

    public async Task<List<CustomerTable>> GetExcelCustomer(string search, DateTime? fromDate, DateTime? toDate)
    {
        var custList = await _customerRepository.CustomerExcel(search, fromDate, toDate);

        var customers = await custList
        .Select(o => new CustomerTable
        {
            Customerid = o.Customerid,
            Customername = o.Customername,
            Email = o.Email,
            Phoneno = o.Phoneno,
            Orderdate = o.Createdat,
            Totalorder = o.Totalorder
        })
        .ToListAsync();


        return customers;
    }

    public async Task<CustomerTable> CustomerHistory(int id)
    {
        var customer = await _customerRepository.CustomerHistory(id);
      
        List<OrderHistory> orders = customer.Orders.Select(o => new OrderHistory
        {
            Orderid = o.Orderid,
            Orderdate = o.Orderdate,
            TotalAmount = o.Totalamount,
            paymentmode = (Entity.Models.Enums.paymentmode)o.Paymentmode,
            orderType = 1,
            Quantity = (short)o.Ordereditems.Count()
        }).ToList();

        var comingSince = customer.Orders.Min(o => o.Orderdate);
        decimal Avgbill = customer.Orders.Count() > 0 ? customer.Orders.Average(o => o.Totalamount) : 0 ;
        var MaxOrder = customer.Orders.Count() > 0 ? customer.Orders.Max(o => o.Totalamount) : 0;


       
       var customerHistory = new CustomerTable
        {
            Customerid = customer.Customerid,
            Customername = customer.Customername,
            Phoneno = customer.Phoneno,
            MaxOrder = (int)MaxOrder,
            orders = orders,
            Cominng_Since = comingSince,
            AvgBill = Math.Round(Avgbill,2),
            VisitCount = customer.Visitcount
       };

        return customerHistory;
    }

}
