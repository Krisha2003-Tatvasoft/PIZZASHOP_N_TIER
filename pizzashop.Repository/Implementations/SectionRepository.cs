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
        return await  _context.Sections.Where(c=>c.Isdeleted == false)
        .OrderBy(c=> c.Sectionid).ToListAsync();
    }


}
