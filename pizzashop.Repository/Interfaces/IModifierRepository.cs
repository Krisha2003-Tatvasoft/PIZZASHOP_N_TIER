using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifierRepository
{
    List<Modifier> GetModifierByMG(int id);
}
