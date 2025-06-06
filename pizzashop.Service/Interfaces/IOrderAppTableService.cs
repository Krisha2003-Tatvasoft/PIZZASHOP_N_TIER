using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppTableService
{
    Task<AssignTable> AssignTable(List<int> id);

    Task<(int? orderid, string Message)> AssignTablePost(int loginid , AssignTable model);
}
