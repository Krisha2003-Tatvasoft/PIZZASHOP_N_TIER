using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ITableRepository
{
    Task<List<Table>> GetTablesySec(int id);
}
