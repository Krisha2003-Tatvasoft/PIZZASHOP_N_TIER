using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

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
        return await _context.Sections.AnyAsync(s => s.Sectionname.ToLower() == sectioname.ToLower());
    }

    public async Task<Section> SecByIdAsync(int id)
    {
        return await _context.Sections.FirstOrDefaultAsync(u => u.Sectionid == id);
    }

    public async Task<bool> SecNameExistAtEditAsync(string sectionname, int id)
    {
        return await _context.Sections.AnyAsync(s => s.Sectionname.ToLower() == sectionname.ToLower() && s.Sectionid != id);
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


}
