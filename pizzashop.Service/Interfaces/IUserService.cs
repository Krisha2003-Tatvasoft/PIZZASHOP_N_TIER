using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IUserService
{
    Task<(List<UserTable>, int totalUsers)> GetUserTable(int page, int pageSize, string search, string SortColumn, string SortOrder);

    Task<AddNewUser> GetAddNewUser();

    Task PostAddNewUser(AddNewUser model, int loginId);

    Task<AddNewUser> GetUpdate(int id);

    Task<bool> PostUpdate(AddNewUser model);

    Task<bool> Delete(int id);

    Task<bool> emailExist(string email);

    Task<bool> usernameExist(string Username);

    Task<bool> phoneExist(string phone);

    Task<List<SelectListItem>> GetRolesAsync();

    Task<bool> usernameExistEdit(string Username, int userid);

    Task<bool> phoneExistEdit(string phone, int userid);

    Task<Userslogin> GetUserLoginByEmail(string email);

    Task UpdateUserLogin(Userslogin userlogin);

    string GeneratePassword(int length = 8);

}
