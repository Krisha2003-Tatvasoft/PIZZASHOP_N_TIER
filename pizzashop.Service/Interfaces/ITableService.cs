namespace pizzashop.Service.Interfaces;
using VMTable = pizzashop.Entity.ViewModels.Table;
using VMAddTable = pizzashop.Entity.ViewModels.AddTable;
using Microsoft.AspNetCore.Mvc.Rendering;


public interface ITableService
{
  Task<(List<VMTable>, int totalTables)> GetTableBySec(int id, int page, int pageSize, string search);

  Task<VMAddTable> AddTable();

  Task<bool> AddTablePost(int loginId, VMAddTable viewmodel);

  Task<VMAddTable> EditTable(int id);

  Task<bool> EditTablePost(int loginId, VMAddTable viewmodel);

  Task<bool> DeleteTable(int id);

  Task<bool> DeleteSelectedTable(List<int> selectedIds);

  Task<List<SelectListItem>> GetTablesListDD(int sectionId);

  Task<bool> TableOccupeidBySec(int id);
  

}
