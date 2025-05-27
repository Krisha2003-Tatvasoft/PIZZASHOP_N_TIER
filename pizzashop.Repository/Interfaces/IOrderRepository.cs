using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Repository.Interfaces;

public interface IOrderRepository
{
    Task<IQueryable<Order>> OrderTable(string search, string SortColumn, string SortOrder, string status,
    DateTime? fromDate, DateTime? toDate);

    Task<IQueryable<Order>> OrderExcelTable(string search, string status, DateTime? fromDate, DateTime? toDate);

    Task<Order> OrderDetailsByIdAsync(int id);

    Task AddNewOrder(Order order);

    Task<List<Order>> InprogressOrders();

    Task UpdateItemStatusAsync(OrderItemStatus model);

    Task UpdateOrder(Order order);

    Task<DashboardViewModel> GetDashboardData(DateTime? fromDate, DateTime? toDate);

    Task<List<TicketResult>> GetTicketResultFromSP(int categoryId, int status);

    Task<List<TicketDetailResult>> GetTicketDetailsFromSP(int orderId, int status, int categoryId);

    Task<List<OrderDetailsFlat>> GetOrderDetailsSp(int orderId);

    Task<List<TaxTable>> GetAllTaxEnabledSp();

    Task<(bool Success, string Message)> SaveOrder_SP(int orderId, List<OrderItem> items, List<OrderTax> taxes);

    Task<(bool Success, string Message)> CancelOrder_SP(int orderId);

    Task<(bool Success, string Message)> CompleteOrder_SP(int orderId);

    Task<(bool Success, string Message)> AddReview_SP(Review model);

    Task<CustomerDetail?> GetCustomerDetail_SP(int orderId);

    Task<(bool success, string message)> EditCustomerDetail_SP(CustomerDetail model);

    Task<Order> GetOrderBYId_SQL(int ordrid);


    Task<bool> AddOrderComment_SP(int orderId, string comment);

}
