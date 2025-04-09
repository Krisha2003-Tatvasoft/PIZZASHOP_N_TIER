using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppTableService
{
    Task<AssignTable> AssignTable(int id);

    Task<bool> AssignTablePost(int loginid , AssignTable model);
}
