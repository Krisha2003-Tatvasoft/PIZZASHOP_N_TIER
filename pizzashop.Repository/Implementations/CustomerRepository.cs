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
        
        var custList = _context.Customers
        .Include(o => o.Orders)
        .Where(o =>  (!fromDate.HasValue || o.Createdat >= fromDate) &&
            (!toDate.HasValue || o.Createdat <= toDate) )
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

}
