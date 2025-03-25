using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;


namespace pizzashop.Repository.Implementations;

public class ItemmodifiergroupmapRepository : IItemmodifiergroupmapRepository
{
    private readonly PizzashopContext _context;

    public ItemmodifiergroupmapRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddNewMapping(Itemmodifiergroupmap itemmodifiergroupmap)
    {
        _context.Itemmodifiergroupmaps.Add(itemmodifiergroupmap);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Itemmodifiergroupmap>> GetMGMByitemid(int id)
    {
       return  await _context.Itemmodifiergroupmaps.Include(c=>c.Modifiergroup)
        .Where(img => img.Itemid == id).ToListAsync();
    }

    public async Task UpdateMapping(Itemmodifiergroupmap mapping)
    {
        _context.Itemmodifiergroupmaps.Update(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMapping(Itemmodifiergroupmap mapping)
    {
        _context.Itemmodifiergroupmaps.Remove(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMappingByItem(int id)
    {
        var mappings = _context.Itemmodifiergroupmaps.Where(m => m.Itemid == id);
        _context.Itemmodifiergroupmaps.RemoveRange(mappings);
         await _context.SaveChangesAsync();
    }

    
   

}
