using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppMenuService
{
    Task<List<IMGMviewmodel>> ModifiersById(int id);

    Task<Bill?> OrderDetails(int id);
}
