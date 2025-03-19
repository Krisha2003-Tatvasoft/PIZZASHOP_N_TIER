using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class TableRepository : ITableRepository
{

  private readonly PizzashopContext _context;

    public TableRepository(PizzashopContext context)
    {
        _context = context;
    }

     public async Task<List<Table>> GetTablesySec(int id)
    {
       
        return await _context.Tables
        .Where(c => c.Sectionid == id && c.Isdeleted == false)
        .OrderBy(c => c.Tableid).ToListAsync();
    }


}
