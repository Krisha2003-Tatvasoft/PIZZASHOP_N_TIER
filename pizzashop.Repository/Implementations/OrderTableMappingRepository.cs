using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class OrderTableMappingRepository : IOrderTableMappingRepository
{
    private readonly PizzashopContext _context;

    public OrderTableMappingRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddNewOrderMapping(Ordertable mapping)
    {
        _context.Ordertables.Add(mapping);
        await _context.SaveChangesAsync();
    }



}
