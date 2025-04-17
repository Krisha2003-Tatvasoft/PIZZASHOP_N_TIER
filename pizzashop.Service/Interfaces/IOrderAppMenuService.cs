using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppMenuService
{
    Task<List<IMGMviewmodel>> ModifiersById(int id);
}
