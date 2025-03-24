using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ITaxesRepository
{
 Task<List<Taxis>> GetTaxTableAsync(string search);
}
