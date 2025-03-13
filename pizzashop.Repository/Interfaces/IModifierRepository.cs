using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IModifierRepository
{
    Task<List<Modifier>> GetModifierByMG(int id);
}
