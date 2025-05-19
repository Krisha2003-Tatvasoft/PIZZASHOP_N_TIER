using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppMenuService
{
    Task<List<IMGMviewmodel>> ModifiersById(int id);

    Task<Bill?> OrderDetails(int id);

    Task<(bool success, string message)> SaveOrder(Bill model);

    Task<string?> GetOrderComment(int id);

    Task<bool> AddOrderComment(string comment, int orderid);

    Task<(bool success, string message)> CancelOrder(int orderid);

    Task<(bool success, string message)> CompleteOrder(int orderid);

    Task<CustomerDetail?> CustomerDetail(int orderid);

    Task<(bool sucess, string message)> EditCustomerdetail(CustomerDetail model);

    Task<(bool sucess, string message)> ReviewPost(Review model);

    Task<bool> checkOrderStatus(int id);
}
