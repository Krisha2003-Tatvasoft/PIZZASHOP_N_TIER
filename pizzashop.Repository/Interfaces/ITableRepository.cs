using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ITableRepository
{
    Task<List<Table>> GetTablesySec(int id, string search);

    Task<Table> TableByIdAsync(int id);

     Task<bool> DeleteBySection(int id);

      Task<bool> TableNameExist(int? sectionId, string tableName);

      Task AddNewTable(Table table);

      Task<bool> TableNameExistInEdit(int? sectionId, string tableName,int tableId);

      Task UpdateTable(Table table);

      Task DeleteTable(Table table);

      Task DeleteSelected(List<int> SelectedIds);

      Task<List<SelectListItem>> TableDDAsync(int sectionId);

}
