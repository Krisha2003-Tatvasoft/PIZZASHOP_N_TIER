using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly PizzashopContext _context;

    public CategoryRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task addAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public List<Category> AllCategory()
    {
          return  _context.Categories.ToList();
    }
    
}
