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
}
