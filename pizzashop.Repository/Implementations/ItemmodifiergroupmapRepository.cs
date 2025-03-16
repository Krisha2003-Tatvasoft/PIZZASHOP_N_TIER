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
}
