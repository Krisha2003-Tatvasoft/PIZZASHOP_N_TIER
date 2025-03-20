namespace pizzashop.Service.Interfaces;
using VMTable = pizzashop.Entity.ViewModels.Table;
using VMAddTable = pizzashop.Entity.ViewModels.AddTable;

public interface ITableService
{
  Task<List<VMTable>> GetTableBySec(int id);

  Task<VMAddTable> AddTable();

  Task<bool> AddTablePost(int loginId, VMAddTable viewmodel);

  Task<VMAddTable> EditTable(int id);

  Task<bool> EditTablePost(int loginId, VMAddTable viewmodel);

  Task<bool> DeleteTable(int id);
  
}
