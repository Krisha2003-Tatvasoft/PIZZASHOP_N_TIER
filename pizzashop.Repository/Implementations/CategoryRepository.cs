using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;
using VMCategory = pizzashop.Entity.ViewModels.Category;
using Dapper;
using Npgsql;
using System.Data;

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

    public async Task<List<Category>> AllCategory()
    {
        return await _context.Categories.Where(c => c.Isdeleted == false).OrderBy(c => c.sortOrder).ToListAsync();
    }

    public async Task<Category> GetCatById(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Categoryid == id);
    }

    public async Task<List<Category>> AllCategoryForOrder()
    {
        return await _context.Categories.Where(c => c.Isdeleted == false).OrderBy(c => c.Categoryid).ToListAsync();
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
        else
        {
            return false;
        }

    }


    public async Task<List<SelectListItem>> GetAllCatyAsync() =>
       await _context.Categories.Where(c => c.Isdeleted == false).Select
       (c => new SelectListItem
       { Value = c.Categoryid.ToString(), Text = c.Categoryname })
       .ToListAsync();



    public async Task<bool> CatExistAsync(string Categoryname)
    {
        return await _context.Categories.AnyAsync(c => c.Categoryname.ToLower().Trim() == Categoryname.ToLower().Trim() && c.Isdeleted == false);
    }

    public async Task<bool> CatNameExistAtEditAsync(string Categoryname, int id)
    {
        return await _context.Categories.AnyAsync(c => c.Categoryname.ToLower().Trim() == Categoryname.ToLower().Trim() && c.Categoryid != id && c.Isdeleted == false);
    }

    public async Task<List<VMCategory>> GetKOTCategoryListFromSP(int status)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<VMCategory>(
            "SELECT * FROM get_kot_category_list(@status);",
            new { status }
        );

        return result.ToList();
    }


    public async Task<List<VMCategory>> GetMenuCategoryListSP()
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<VMCategory>(
            "SELECT * FROM get_menu_category_list();" );

        return result.ToList();
    }


}
