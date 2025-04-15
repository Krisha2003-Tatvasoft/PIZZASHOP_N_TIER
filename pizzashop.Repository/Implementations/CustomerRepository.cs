using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class CustomerRepository : ICustomerRepository
{

    private readonly PizzashopContext _context;

    public CustomerRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Customer>> CustomerTable(string search, string SortColumn, string SortOrder, DateTime? fromDate, DateTime? toDate)
    {
        string lowerSearch = search.ToLower();
        toDate = toDate.HasValue ? toDate.Value.AddDays(1).AddTicks(-1) : toDate;
        var custList = _context.Customers
        .Include(o => o.Orders)
        .Where(o =>
        (o.Orders.Any() &&
        (!fromDate.HasValue || o.Orders.Any(order => order.Orderdate >= fromDate)) &&
        (!toDate.HasValue || o.Orders.Any(order => order.Orderdate <= toDate))) ||
        (!o.Orders.Any() &&
        (!fromDate.HasValue || o.Createdat >= fromDate) &&
         (!toDate.HasValue || o.Createdat <= toDate)))
        .Where(u => string.IsNullOrEmpty(lowerSearch) ||
                            u.Customername.ToLower().Contains(lowerSearch) ||
                            u.Email.ToLower().Contains(lowerSearch) ||
                            u.Phoneno.ToLower().Contains(lowerSearch) ||
                            u.Createdat.ToString().ToLower().Contains(lowerSearch) ||
                            u.Totalorder.ToString().ToLower().Contains(lowerSearch));

        custList = SortColumn switch
        {
            "CustomerName" => SortOrder == "desc" ? custList.OrderByDescending(o => o.Customername) : custList.OrderBy(o => o.Customername),
            "OrderDate" => SortOrder == "desc" ? custList.OrderByDescending(o => o.Createdat) : custList.OrderBy(o => o.Createdat),
            "TotalOrder" => SortOrder == "desc" ? custList.OrderByDescending(o => o.Totalorder) : custList.OrderBy(o => o.Totalorder),
            // Add additional cases for other columns as needed
            _ => custList.OrderBy(o => o.Customername),// Default sort (if none provided)
        };
        return custList;
    }


    public async Task<IQueryable<Customer>> CustomerExcel(string search, DateTime? fromDate, DateTime? toDate)
    {

        string lowerSearch = search.ToLower();
        toDate = toDate.HasValue ? toDate.Value.AddDays(1).AddTicks(-1) : toDate;

        var custList = _context.Customers
        .Include(o => o.Orders)
       .Where(o =>
        (o.Orders.Any() &&
        (!fromDate.HasValue || o.Orders.Any(order => order.Orderdate >= fromDate)) &&
        (!toDate.HasValue || o.Orders.Any(order => order.Orderdate <= toDate))) ||
        (!o.Orders.Any() &&
        (!fromDate.HasValue || o.Createdat >= fromDate) &&
         (!toDate.HasValue || o.Createdat <= toDate)))
        .Where(u => string.IsNullOrEmpty(lowerSearch) ||
                            u.Customername.ToLower().Contains(lowerSearch) ||
                            u.Email.ToLower().Contains(lowerSearch) ||
                            u.Phoneno.ToLower().Contains(lowerSearch) ||
                            u.Createdat.ToString().ToLower().Contains(lowerSearch) ||
                            u.Totalorder.ToString().ToLower().Contains(lowerSearch));

        return custList;
    }


    public async Task<Customer> CustomerHistory(int id)
    {
        return await _context.Customers
           .Include(o => o.Orders).ThenInclude(o => o.Ordereditems)
            .FirstOrDefaultAsync(o => o.Customerid == id);
    }

    public async Task<Customer> GetCustomerByEmail(string email)
    {
        return await _context.Customers
        .Include(c =>c.Orders).FirstOrDefaultAsync(o => o.Email == email);
    }

    public async Task<Customer> GetCustomerById(int? id)
    {
        return await _context.Customers.FirstOrDefaultAsync(o => o.Customerid == id);
    }


    public async Task AddNewCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Customer customer)
    {
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }





}
