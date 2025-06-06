using pizzashop.Entity.ViewModels;


namespace pizzashop.Service.Interfaces;

public interface IOrderService
{

    Task<(List<OrderTable>, int totalOrder)> GetOrderTable(int page, int pageSize, string search,
    string SortColumn, string SortOrder, string status, DateTime? fromDate, DateTime? toDate);

    Task<List<OrderTable>> GetExcelOrderTable(string search,
        string status, DateTime? fromDate, DateTime? toDate);

    Task<OrderDetails> OrderDetails(int id);

    

}
