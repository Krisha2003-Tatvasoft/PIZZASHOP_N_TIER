using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IOrderTableMappingRepository
{
    Task AddNewOrderMapping(Ordertable mapping);
}
