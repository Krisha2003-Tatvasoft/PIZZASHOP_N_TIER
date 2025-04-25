using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppMenuService
{
    Task<List<IMGMviewmodel>> ModifiersById(int id);

    Task<Bill?> OrderDetails(int id);

    bool AreModifiersSame(List<Ordereditemmodifer> existingModifiers, List<OrderModifier> newModifiers);

    Task<(bool success, string message)> SaveOrder(Bill model);
}
