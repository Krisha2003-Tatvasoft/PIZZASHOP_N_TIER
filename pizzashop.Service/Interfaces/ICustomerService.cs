using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface ICustomerService
{

     Task<(List<CustomerTable>, int totalCustomer)> GetCustomerTable(int page, int pageSize, string search,
   string SortColumn, string SortOrder, DateTime? fromDate, DateTime? toDate);

   Task<List<CustomerTable>> GetExcelCustomer(string search, DateTime? fromDate, DateTime? toDate);

   Task<CustomerTable> CustomerHistory(int id);

    Task<Customer?> GetCustomerByEmail (string email);
}
