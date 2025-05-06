using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IOrderAppWaitingTokenService
{
    Task<AddWaitingToken> AddWaitingToken(int id);

   Task<(bool Success, string Message)> AddWaitingTokenPost(int loginId, AddWaitingToken model);

    Task<List<WaitingListTable>> WaitingListBySectionId(List<int> sectionId);

    Task<AssignTable> DetailsFromWT(int id);

    Task<List<WaitingListTable>> WaitingList(int sectionId);

    Task<AddWaitingToken> EditGetWT(int id);

    // Task<bool> EmailExistsWithId(string email , int id);

    Task<bool> EditPosttWT(int loginId,AddWaitingToken model);

    Task<bool> DeleteWT(int id);

  Task<(int? orderid, string Message)> AssignTablePost(int loginid, WaitingListAssignTable model);
}
