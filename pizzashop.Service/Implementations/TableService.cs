using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using VMTable = pizzashop.Entity.ViewModels.Table;

namespace pizzashop.Service.Implementations;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<List<VMTable>> GetTableBySec(int id)
    {
        List<Table> tables = await _tableRepository.GetTablesySec(id);

        var tableList = tables
        .Select(t => new VMTable
        {
            Tablename = t.Tablename,
            Capacity = t.Capacity,
            tablestatus = t.tablestatus
        }
        ).ToList();

        return tableList;

    }
}
