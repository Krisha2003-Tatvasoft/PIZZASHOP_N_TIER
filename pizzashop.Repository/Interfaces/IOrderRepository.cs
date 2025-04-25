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
}
