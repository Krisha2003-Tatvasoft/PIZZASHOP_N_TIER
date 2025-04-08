using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IWaitingTokenRepository
{
    Task AddNewWaitingToken(Waitingtoken token);
}
