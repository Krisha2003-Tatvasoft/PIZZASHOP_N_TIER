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
        var custList = await _customerRepository.CustomerTable(search, SortColumn, SortOrder,fromDate, toDate);

        int totalCustomer = await custList.CountAsync();

        var orders = await custList
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
       

        return (orders, totalCustomer);
    }

}
