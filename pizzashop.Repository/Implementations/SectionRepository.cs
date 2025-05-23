using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using VMTable = pizzashop.Entity.ViewModels.Table;
using pizzashop.Repository.Interfaces;
using VMSectionWIthCount = pizzashop.Entity.ViewModels.SectionWithCount;
using VMOrderTableView = pizzashop.Entity.ViewModels.OrderTableView;
using RawOrderTableView = pizzashop.Entity.ViewModels.RawOrderTableView;
using Dapper;
using Npgsql;
using static pizzashop.Entity.Models.Enums;


namespace pizzashop.Repository.Implementations;

public class SectionRepository : ISectionRepository
{
    private readonly PizzashopContext _context;

    public SectionRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Section>> AllSections()
    {
        return await _context.Sections.Where(c => c.Isdeleted == false)
        .OrderBy(c => c.sortOrder).ToListAsync();
    }


    public async Task<List<Section>> AllSectionsorder()
    {
        return await _context.Sections.Where(c => c.Isdeleted == false)
        .OrderBy(c => c.Sectionid).ToListAsync();
    }



    public async Task AddNewSection(Section section)
    {
        _context.Sections.Add(section);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SectionNameAsync(string sectioname)
    {
        return await _context.Sections.AnyAsync(s => s.Sectionname.ToLower().Trim() == sectioname.ToLower().Trim() && s.Isdeleted == false);
    }

    public async Task<Section> SecByIdAsync(int id)
    {
        return await _context.Sections.FirstOrDefaultAsync(u => u.Sectionid == id);
    }

    public async Task<bool> SecNameExistAtEditAsync(string sectionname, int id)
    {
        return await _context.Sections.AnyAsync(s => s.Sectionname.ToLower().Trim() == sectionname.ToLower().Trim() && s.Sectionid != id && s.Isdeleted == false);
    }

    public async Task UpdateSection(Section section)
    {
        _context.Sections.Update(section);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSection(Section section)
    {
        section.Isdeleted = true;
        _context.Sections.Update(section);
        await _context.SaveChangesAsync();
    }


    public async Task<List<SelectListItem>> SectionDDAsync() =>
      await _context.Sections
      .Where(c => c.Isdeleted == false).Select
      (c => new SelectListItem
      { Value = c.Sectionid.ToString(), Text = c.Sectionname })
      .ToListAsync();


    public async Task<List<Section>> GetSectionWithTables()
    {
        return await _context.Sections.Where(s => s.Isdeleted == false)
        .Include(s => s.Tables.Where(t => t.Isdeleted == false)).ThenInclude(t => t.Ordertables).ThenInclude(s => s.Order)
        .ToListAsync();
    }

    public async Task<List<SelectListItem>> SectionDDAvailable() =>
      await _context.Sections
      .Where(c => c.Isdeleted == false && c.Tables.Any(t => t.tablestatus == 0 && t.Isdeleted == false)).Select
      (c => new SelectListItem
      { Value = c.Sectionid.ToString(), Text = c.Sectionname })
      .ToListAsync();

    public async Task<List<VMSectionWIthCount>> GetWTSectionListFromSP()
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();


        var result = await connection.QueryAsync<VMSectionWIthCount>(
            "SELECT * FROM get_sections_with_token_count()"
        );

        return result.ToList();
    }


    public async Task<List<SelectListItem>> SectionDDFromSp()
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<VMSectionWIthCount>(
            "SELECT * FROM get_sections_with_token_count()"
        );

        return result.Where(s => s.Sectionid != 0).Select(s => new SelectListItem
        {
            Value = s.Sectionid.ToString(),
            Text = s.Sectionname
        }).ToList();
    }

    public async Task<List<VMOrderTableView>> GetOrderTableViewsFromSP()
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var rawData = await connection.QueryAsync<RawOrderTableView>(
            "SELECT * FROM get_orderapp_table_views();");

        var grouped = rawData
            .GroupBy(r => new { r.Sectionid, r.Sectionname })
            .Select(g => new VMOrderTableView
            {
                Sectionid = g.Key.Sectionid,
                Sectionname = g.Key.Sectionname,
                Tables = g.Select(t => new VMTable
                {
                    Tableid = t.Tableid,
                    Tablename = t.Tablename,
                    Sectionid = g.Key.Sectionid,
                    Capacity = t.Capacity,
                    tablestatus = (tablestatus)t.tablestatus,
                    orderstatus = (orderstatus)t.orderstatus,
                    Totalamount = t.Totalamount,
                    RunningSince = t.RunningSince,
                    Orderid = t.Orderid
                }).ToList()
            })
            .ToList();

        return grouped;
    }


}
