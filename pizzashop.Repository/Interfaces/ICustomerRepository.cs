using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ICustomerRepository
{
    Task<IQueryable<Customer>> CustomerTable(string search, string SortColumn, string SortOrder, DateTime? fromDate, DateTime? toDate);
}
