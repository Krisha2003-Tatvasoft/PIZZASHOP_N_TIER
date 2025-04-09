using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppWaitingTokenService
{
    Task<AddWaitingToken> AddWaitingToken(int id);

    Task<bool> AddWaitingTokenPost(int loginId, AddWaitingToken model);

    Task<List<WaitingListTable>> WaitingListBySectionId(int sectionId);

    Task<AssignTable> DetailsFromWT(int id);
}
