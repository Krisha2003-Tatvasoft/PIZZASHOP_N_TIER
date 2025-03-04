using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IUserService
{
     Task<(List<UserTable>,int totalUsers)> GetUserTable(int page,int pageSize, string search);
    
    Task<AddNewUser> GetAddNewUser();

    Task<bool> PostAddNewUser(AddNewUser model,int loginId);

    Task<AddNewUser> GetUpdate(int id);
  
     Task<bool> PostUpdate(AddNewUser model);

      Task<bool> Delete(int id);

}
