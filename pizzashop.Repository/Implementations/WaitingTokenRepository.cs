using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class WaitingTokenRepository : IWaitingTokenRepository
{
    
    private readonly PizzashopContext _context;

    public WaitingTokenRepository(PizzashopContext context)
    {
        _context = context;
    }

   public async Task AddNewWaitingToken(Waitingtoken token)
    {
        _context.Waitingtokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Waitingtoken>> WaitingListBySectionId(int sectionId)
    {
        return await _context.Waitingtokens
            .Include(w => w.Customer)
            .Include(w => w.Section)
            .Where(w => w.Sectionid == sectionId  && w.Isassigned == false)
            .ToListAsync();
    }

     public async Task<Waitingtoken> WTByIdAsync(int? id)
    {
        return await _context.Waitingtokens
        .Include(w => w.Customer)
        .Include(w => w.Section)
        .FirstOrDefaultAsync(wt => wt.Waitingtokenid == id);
    }

    public async Task UpdateWaitingToken(Waitingtoken waitingToken)
    {
        _context.Waitingtokens.Update(waitingToken);
        await _context.SaveChangesAsync();
    }


}
