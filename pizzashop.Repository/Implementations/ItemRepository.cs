using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;
using static pizzashop.Entity.Models.Enums;
using IMGMviewmodel = pizzashop.Entity.ViewModels.IMGMviewmodel;

namespace pizzashop.Repository.Implementations;

public class ItemRepository : IItemRepository
{
    private readonly PizzashopContext _context;

    public ItemRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Item>> GetItemByCat(int id, string search)
    {
        string lowerSearch = search.ToLower();
        return await _context.Items
        .Include(u => u.Unit)
        .Where(c => c.Categoryid == id && c.Isdeleted == false)
        .Where(c => string.IsNullOrEmpty(lowerSearch) ||
         c.Itemname.ToLower().Contains(lowerSearch) ||
          c.Quantity.ToString().ToLower().Contains(lowerSearch) ||
           c.Rate.ToString().ToLower().Contains(lowerSearch))
        .OrderBy(c => c.Itemid).ToListAsync();
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

    public async Task DeleteSelected(List<int> SelectedIds)
    {
        await _context.Items
       .Where(i => SelectedIds.Contains(i.Itemid))
       .ExecuteUpdateAsync(s => s.SetProperty(i => i.Isdeleted, true));

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ItemExistAsync(string Itemname)
    {
        return await _context.Items.AnyAsync(c => c.Itemname.ToLower().Trim() == Itemname.ToLower().Trim() && c.Isdeleted == false);
    }

    public async Task<bool> ItemNameExistAtEditAsync(string Itemname, int id)
    {
        return await _context.Items.AnyAsync(c => c.Itemname.ToLower().Trim() == Itemname.ToLower().Trim() && c.Itemid != id && c.Isdeleted == false);
    }

    public async Task<List<Item>> GetMenuItem(string search)
    {
        string lowerSearch = search.ToLower();
        return await _context.Items
        .Include(u => u.Unit)
        .Where(c => c.Isdeleted == false && c.Isavailable == true)
        .Where(c => string.IsNullOrEmpty(lowerSearch) ||
         c.Itemname.ToLower().Contains(lowerSearch) ||
          c.Quantity.ToString().ToLower().Contains(lowerSearch) ||
           c.Rate.ToString().ToLower().Contains(lowerSearch))
        .OrderBy(c => c.Itemid).ToListAsync();
    }


    public async Task<Item> ItemWithModifier(int id)
    {

        return await _context.Items
        .Include(i => i.Itemmodifiergroupmaps)
        .ThenInclude(im => im.Modifiergroup)
        .ThenInclude(mg => mg.ModifierGroupModifier)
        .ThenInclude(m => m.Modifier)
        .FirstOrDefaultAsync(i => i.Itemid == id);

    }

    public async Task<List<Item>> GetMenuItemSP(string search, int categoryId)
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<Item>(
            "SELECT * FROM get_menu_itemlist (@p_search, @p_categoryid);",
            new { p_search = search, p_categoryid = categoryId }
        );

        return result.ToList();
    }

    public async Task<List<IMGMviewmodel>> ModifiersByIdSP(int itemId)
    {
        using var conn = new NpgsqlConnection(_context.Database.GetConnectionString());

        var result = await conn.QueryAsync<dynamic>(
            "SELECT * FROM get_modifiers_by_item_id(@itemid);",
            new { itemid = itemId });

        var grouped = result
            .GroupBy(r => new
            {
                Itemid = (int)r.itemid,
                Itemname = (string)r.itemname,
                Itemmodifiergroupid = (int)r.itemmodifiergroupid,
                Modifiergroupid = (int)r.modifiergroupid,
                Modifiergroupname = (string)r.modifiergroupname,
                Minselectionrequired = (short?)r.minselectionrequired,
                Maxselectionallowed = (short?)r.maxselectionallowed,
                Itemtype = (itemtype)(int)r.itemtype
            })
            .Select(g => new IMGMviewmodel
            {
                Itemid = g.Key.Itemid,
                Itemname = g.Key.Itemname,
                Itemmodifiergroupid = g.Key.Itemmodifiergroupid,
                Modifiergroupid = g.Key.Modifiergroupid,
                Modifiergroupname = g.Key.Modifiergroupname,
                Minselectionrequired = g.Key.Minselectionrequired,
                Maxselectionallowed = g.Key.Maxselectionallowed,
                itemtype = g.Key.Itemtype,
                modifiers = g.Select(x => new Modifier
                {
                    Modifierid = x.modifierid,
                    Modifiername = x.modifiername,
                    Rate = (decimal)x.rate
                }).ToList()
            }).ToList();

        return grouped;
    }






}
