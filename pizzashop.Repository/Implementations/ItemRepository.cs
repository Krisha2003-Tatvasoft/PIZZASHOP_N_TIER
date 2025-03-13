using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class ItemRepository : IItemRepository
{
    private readonly PizzashopContext _context;

    public ItemRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Item>> GetItemByCat(int id)
    {
        return await _context.Items
        .Include(u => u.Unit)
        .Where(c => c.Categoryid == id && c.Isdeleted == false).ToListAsync();
    }

    public async Task<bool> DeleteByCat(int id)
    {
        var Items = _context.Items.Where(c => c.Categoryid == id).ToList();
        if (Items.Count != 0)
        {

            await _context.Items.Where(i => i.Categoryid == id).ForEachAsync(i => i.Isdeleted = true);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task AddNewItemAsync(Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<Item> ItemByIdAsync(int id)
    {
        return await _context.Items.FirstOrDefaultAsync(u => u.Itemid == id);
    }

    public async Task UpdateItem(Item item)
    {
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItem(Item item)
    {
      item.Isdeleted = true;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
    }

}
