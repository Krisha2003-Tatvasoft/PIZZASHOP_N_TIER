using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IOrderTaxRepository
{
    Task<List<Ordertaxmapping>> GetTaxesByOrderIdAsync(int orderId);

    Task AddAsync(Ordertaxmapping orderTax);

    Task UpdateAsync(Ordertaxmapping orderTax);

    Task DeleteAsync(Ordertaxmapping orderTax);
}
