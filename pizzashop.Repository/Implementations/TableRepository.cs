using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class TableRepository : ITableRepository
{

    private readonly PizzashopContext _context;

    public TableRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Table>> GetTablesySec(int id, string search)
    {
        string lowerSearch = search.ToLower();
        return await _context.Tables
        .Where(c => c.Sectionid == id && c.Isdeleted == false)
          .Where(c => string.IsNullOrEmpty(lowerSearch) ||
         c.Tablename.ToLower().Contains(lowerSearch) ||
          c.Capacity.ToString().ToLower().Contains(lowerSearch))
        .OrderBy(c => c.Tableid).ToListAsync();
    }

    public async Task<bool> DeleteBySection(int id)
    {
        var Tables = _context.Tables.Where(c => c.Tableid == id).ToList();
        if (Tables.Count != 0)
        {
            await _context.Tables.Where(i => i.Tableid == id)
            .ForEachAsync(i => i.Isdeleted = true);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<Table> TableByIdAsync(int id)
    {
        return await _context.Tables.FirstOrDefaultAsync(u => u.Tableid == id);
    }

    public async Task<bool> TableNameExist(int? sectionId, string tableName)
    {
        return await _context.Tables.AnyAsync(s => s.Sectionid == sectionId && s.Tablename.ToLower().Trim() == tableName.ToLower().Trim() && s.Isdeleted == false);
    }


    public async Task AddNewTable(Table table)
    {
        _context.Tables.Add(table);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> TableNameExistInEdit(int? sectionId, string tableName, int tableId)
    {
        return await _context.Tables.AnyAsync(s => s.Sectionid == sectionId && s.Tablename.ToLower().Trim() == tableName.ToLower().Trim()
        && s.Tableid != tableId && s.Isdeleted == false);
    }


    public async Task UpdateTable(Table table)
    {
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTable(Table table)
    {
        table.Isdeleted = true;
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSelected(List<int> SelectedIds)
    {
        await _context.Tables
       .Where(i => SelectedIds.Contains(i.Tableid))
       .Where(t => t.tablestatus != Enums.tablestatus.Occupied)
       .ExecuteUpdateAsync(s => s.SetProperty(i => i.Isdeleted, true));

        await _context.SaveChangesAsync();
    }

    public async Task<List<SelectListItem>> TableDDAsync(int sectionId) =>
    await _context.Tables
    .Where(c => c.Isdeleted == false && c.Sectionid == sectionId && c.tablestatus == Enums.tablestatus.Available).Select
    (c => new SelectListItem
    { Value = c.Tableid.ToString(), Text = c.Tablename })
    .ToListAsync();

    public async Task<List<Table>> tablesBysection(int id)
    {
        return await _context.Tables.Where(t => t.Sectionid == id && t.Isdeleted == false).ToListAsync();
    }

    public async Task<List<SelectListItem>> TableDDFromSPAsync(int sectionId)
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        var result = await connection.QueryAsync<SelectListItem>(
            "SELECT * FROM fn_get_available_tables_by_section(@sectionid)",
            new { sectionid = sectionId });

        return result.ToList();
    }

}
