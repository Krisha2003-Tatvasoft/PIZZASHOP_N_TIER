using Microsoft.AspNetCore.Mvc.Rendering;
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
        return _context.Categories.Where(c=>c.Isdeleted == false).ToList();
    }

    public async Task<Category> GetCatById(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Categoryid == id);
    }

    public async Task UpdateCat(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteCat(int id)
    {
        Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Categoryid == id);
        if (category != null)
        {
            category.Isdeleted = true;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }
        else{
            return false;
        }

    }


     public async Task<List<SelectListItem>> GetAllCatyAsync() =>
        await _context.Categories.Select
        (c => new SelectListItem 
        { Value = c.Categoryid.ToString(), Text = c.Categoryname })
        .ToListAsync();

}
