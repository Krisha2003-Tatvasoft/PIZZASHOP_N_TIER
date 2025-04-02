using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface ICustomerExportService
{
      byte[] ExportCustomerToExcel(List<CustomerTable> orders, string searchText,DateTime? fromDate, DateTime? toDate);
}
