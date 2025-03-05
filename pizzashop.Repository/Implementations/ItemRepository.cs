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

    public List<Item> GetItemByCat(int id)
    {
        return _context.Items
        .Include(u =>u.Unit)
        .Where(c=>c.Categoryid==id).ToList();
    }

}
