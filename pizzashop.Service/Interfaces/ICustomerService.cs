using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface ICustomerService
{

     Task<(List<CustomerTable>, int totalCustomer)> GetCustomerTable(int page, int pageSize, string search,
   string SortColumn, string SortOrder, DateTime? fromDate, DateTime? toDate);
}
