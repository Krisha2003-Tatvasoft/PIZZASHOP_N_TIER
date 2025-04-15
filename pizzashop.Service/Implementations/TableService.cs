using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using VMTable = pizzashop.Entity.ViewModels.Table;
using VMAddTable = pizzashop.Entity.ViewModels.AddTable;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.Service.Implementations;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    private readonly ISectionRepository _sectionRepository;

    public TableService(ITableRepository tableRepository, ISectionRepository sectionRepository)
    {
        _tableRepository = tableRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<(List<VMTable>, int totalTables)> GetTableBySec(int id, int page, int pageSize, string search)
    {
        var tables = await _tableRepository.GetTablesySec(id, search);

        int totalTables = tables.Count();

        var tableList = tables
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(t => new VMTable
        {
            Tableid = t.Tableid,
            Tablename = t.Tablename,
            Capacity = t.Capacity,
            tablestatus = t.tablestatus,
            Sectionid = t.Sectionid
        }
        ).ToList();

        return (tableList, totalTables);
    }

    public async Task<VMAddTable> AddTable()
    {
        VMAddTable model = new VMAddTable
        {
            SectionList = await _sectionRepository.SectionDDAsync()
        };
        return model;
    }

    public async Task<bool> AddTablePost(int loginId, VMAddTable viewmodel)
    {
        if (await _tableRepository.TableNameExist(viewmodel.Sectionid, viewmodel.Tablename))
        {
            return false;
        }
        else
        {
            Table table = new Table
            {
                Tablename = viewmodel.Tablename,
                Capacity = viewmodel.Capacity,
                tablestatus = viewmodel.tablestatus,
                Sectionid = viewmodel.Sectionid,
                Createdby = loginId
            };
            await _tableRepository.AddNewTable(table);
            return true;
        }
    }

    public async Task<VMAddTable> EditTable(int id)
    {
        Table table = await _tableRepository.TableByIdAsync(id);
        VMAddTable model = new VMAddTable
        {
            Tableid = table.Tableid,
            Tablename = table.Tablename,
            Capacity = table.Capacity,
            tablestatus = table.tablestatus,
            Sectionid = table.Sectionid,
            SectionList = await _sectionRepository.SectionDDAsync()
        };
        return model;
    }

    public async Task<bool> EditTablePost(int loginId, VMAddTable viewmodel)
    {
        if (await _tableRepository.TableNameExistInEdit(viewmodel.Sectionid, viewmodel.Tablename, viewmodel.Tableid))
        {
            return false;
        }
        else
        {
            Table table = await _tableRepository.TableByIdAsync(viewmodel.Tableid);

            table.Tablename = viewmodel.Tablename;
            table.Capacity = viewmodel.Capacity;
            table.Sectionid = viewmodel.Sectionid;
            table.tablestatus = viewmodel.tablestatus;
            table.Modifiedby = loginId;

            await _tableRepository.UpdateTable(table);
            return true;

        }
    }

    public async Task<bool> DeleteTable(int id)
    {
        Table table = await _tableRepository.TableByIdAsync(id);
        if (table.tablestatus  == Enums.tablestatus.Occupied)
        {
            return false;
        }
        else
        {
            await _tableRepository.DeleteTable(table);
            return true;
        }
    }

    public async Task<bool> DeleteSelectedTable(List<int> selectedIds)
    {
        if (selectedIds.Count == 0)
        {
            return false;
        }

        await _tableRepository.DeleteSelected(selectedIds);
        return true;
    }


    public async Task<List<SelectListItem>> GetTablesListDD(int sectionId)
    {
        return await _tableRepository.TableDDAsync(sectionId);
    }
}
