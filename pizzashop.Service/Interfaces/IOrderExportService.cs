using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderExportService
{

    byte[] ExportOrdersToExcel(List<OrderTable> orders, string searchText, string status, DateTime? fromDate, DateTime? toDate);

}
