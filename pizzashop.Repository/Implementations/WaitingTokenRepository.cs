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


}
