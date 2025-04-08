using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ICustomerRepository
{
    Task<IQueryable<Customer>> CustomerTable(string search, string SortColumn, string SortOrder, DateTime? fromDate, DateTime? toDate);

    Task<IQueryable<Customer>> CustomerExcel(string search, DateTime? fromDate, DateTime? toDate);

    Task<Customer> CustomerHistory(int id);

    Task<Customer> GetCustomerByEmail(string email);

    Task AddNewCustomer(Customer customer);
     Task UpdateCustomer(Customer customer);
}
