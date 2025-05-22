using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Repository.Interfaces;

public interface IWaitingTokenRepository
{
    Task AddNewWaitingToken(Waitingtoken token);

    Task<List<Waitingtoken>> WaitingListBySectionId(int sectionId);

    Task<Waitingtoken> WTByIdAsync(int? id);

    Task UpdateWaitingToken(Waitingtoken waitingToken);

    Task<List<Waitingtoken>> GetWaitingList();

    Task Delete(Waitingtoken waitingToken);

    Task<List<WaitingListTable>> GetWTTokenListFromSP(int sectionId);

    Task<(bool Success, string Message)> AddWaitingTokenPostSP(AddWaitingToken model, int loginId);

    Task<AddWaitingToken> EditGetWTSP(int id);
}
