using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using VMTableView = pizzashop.Entity.ViewModels.OrderTableView;
using pizzashop.Repository.Interfaces;
using VMSectionWIthCount = pizzashop.Entity.ViewModels.SectionWithCount;
using Dapper;


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
        using var connection = _context.Database.GetDbConnection();

        var result = await connection.QueryAsync<VMSectionWIthCount>(
            "SELECT * FROM get_sections_with_token_count()"
        );

        return result.ToList();
    }


    public async Task<List<SelectListItem>> SectionDDFromSp()
    {
        using var connection = _context.Database.GetDbConnection();

        var result = await connection.QueryAsync<VMSectionWIthCount>(
            "SELECT * FROM get_sections_with_token_count()"
        );

        return result.Where(s => s.Sectionid != 0).Select(s => new SelectListItem
        {
            Value = s.Sectionid.ToString(),
            Text = s.Sectionname
        }).ToList();
    }


}
