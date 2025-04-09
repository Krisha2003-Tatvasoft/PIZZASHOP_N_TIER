using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IOrderRepository
{
    Task<IQueryable<Order>> OrderTable(string search, string SortColumn, string SortOrder,string status,
    DateTime? fromDate,DateTime? toDate);

    Task<IQueryable<Order>> OrderExcelTable(string search, string status, DateTime? fromDate, DateTime? toDate);

    Task<Order> OrderDetailsByIdAsync(int id);

    Task AddNewOrder(Order order);
}
