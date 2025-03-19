namespace pizzashop.Service.Interfaces;
using VMTable = pizzashop.Entity.ViewModels.Table;

public interface ITableService
{
  Task<List<VMTable>> GetTableBySec(int id);
  
}
