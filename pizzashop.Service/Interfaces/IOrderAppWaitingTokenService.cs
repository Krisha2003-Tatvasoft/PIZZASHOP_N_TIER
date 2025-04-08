using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppWaitingTokenService
{
    Task<AddWaitingToken> AddWaitingToken(int id);

    Task<bool> AddWaitingTokenPost(int loginId, AddWaitingToken model);
}
