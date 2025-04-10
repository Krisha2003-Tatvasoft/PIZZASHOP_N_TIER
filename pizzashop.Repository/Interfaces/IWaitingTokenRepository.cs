using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface IWaitingTokenRepository
{
    Task AddNewWaitingToken(Waitingtoken token);

     Task<List<Waitingtoken>> WaitingListBySectionId(int sectionId);

      Task<Waitingtoken> WTByIdAsync(int? id);

       Task UpdateWaitingToken(Waitingtoken waitingToken);

        Task<List<Waitingtoken>> GetWaitingList();
}
