using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

namespace pizzashop.Service.Implementations;

public class UserService : IUserService
{
  private readonly IUserRepository _userRepository;

  private readonly ICountryRepository _countryRepository;

  private readonly IStateRepository _stateRepository;
  private readonly ICityRepository _cityRepository;
  private readonly IRoleRepository _roleRepository;

  private readonly IUserDetailsRepository _userDetailsRepository;

  private readonly IFileService _fileService;

  private readonly IHttpContextAccessor _httpContextAccessor;


  public UserService(IUserRepository userRepository, ICountryRepository countryRepository,
  IStateRepository stateRepository, ICityRepository cityRepository, IRoleRepository roleRepository,
   IUserDetailsRepository userDetailsRepository, IFileService fileService, IHttpContextAccessor httpContextAccessor)
  {
    _userRepository = userRepository;
    _countryRepository = countryRepository;
    _stateRepository = stateRepository;
    _cityRepository = cityRepository;
    _roleRepository = roleRepository;
    _userDetailsRepository = userDetailsRepository;
    _fileService = fileService;
    _httpContextAccessor = httpContextAccessor;

  }

  public async Task<(List<UserTable>, int totalUsers)> GetUserTable(int page, int pageSize, string search, string SortColumn, string SortOrder)
  {
    var userList = await _userRepository.GetFilteredAsync(search,SortColumn,SortOrder);
  

    int totalUsers = await userList.CountAsync();

    var users = await userList
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(u => new UserTable
    {
      Email = u.Email,
      Firstname = u.User.Firstname,
      Lastname = u.User.Lastname,
      Phone = u.User.Phone,
      Rolename = u.Role.Rolename,
      userId = (int)u.Userid,
      status = u.status,
      Profileimg = u.User.Profileimg != null
            ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/{u.User.Profileimg}"
            : null
    })
    .ToListAsync();

    return (users, totalUsers);
  }

  public async Task<AddNewUser> GetAddNewUser()
  {
    var model = new AddNewUser
    {
      Countries = await _countryRepository.GetAllCountryAsync(),
      States = new List<SelectListItem>(),
      Cities = new List<SelectListItem>(),
      Roles = await _roleRepository.GetAllRolesAsync()
    };
    return model;
  }

  public async Task<bool> PostAddNewUser(AddNewUser model, int loginId)
  {

    if (await _userRepository.UserExistsAsync(model.Email))
    {
      return false;
    }
    else
    {

      string uniqueFileName = null;
      if (model.ProfilePicture != null)
      {
        uniqueFileName = await _fileService.UploadFileAsync(model.ProfilePicture, "uploads");
      }
      var newUser = new User
      {
        Firstname = model.Firstname,
        Lastname = model.Lastname,
        Phone = model.Phone,
        Countryid = model.Countryid,
        Stateid = model.Stateid,
        Cityid = model.Cityid,
        Address = model.Address,
        Zipcode = model.Zipcode,
        Createdby = loginId,
        Modifiedby = loginId,
        Profileimg = uniqueFileName 
      };

      var hashedPassword = PasswordUtills.HashPassword(model.Password);

      await _userDetailsRepository.AddUser(newUser);

      var newUserLogin = new Userslogin
      {
        Email = model.Email,
        Passwordhash = hashedPassword,
        Userid = newUser.Userid,
        Roleid = model.Roleid,
        Username = model.Username
      };

      await _userRepository.AddNewUser(newUserLogin);

      return true;
    }
  }

  public async Task<AddNewUser> GetUpdate(int id)
  {
    var user = await _userRepository.GetUserByIdAsync(id);
    var modal = new AddNewUser
    {
      Email = user.Email,
      Username = user.Username,
      Userid = user.Userid,
      Firstname = user.User.Firstname,
      Lastname = user.User.Lastname,
      Phone = user.User.Phone,
      Countryid = user.User.Countryid,
      Stateid = user.User.Stateid,
      Cityid = user.User.Cityid,
      Address = user.User.Address,
      Zipcode = user.User.Zipcode,
      Roleid = user.Role.Roleid,
      Password = user.Passwordhash,
       Profileimg = user.User.Profileimg != null
            ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/{ user.User.Profileimg }"
            : null,
      Countries = await _countryRepository.GetAllCountryAsync(),
      States = await _stateRepository.GetStatesByCountryAsync(user.User.Countryid),
      Cities = await _cityRepository.GetCitiesByStateAsync(user.User.Stateid),
      Roles = await _roleRepository.GetAllRolesAsync(),
      status = user.status,
      
    };
    return modal;
  }

  public async Task<bool> PostUpdate(AddNewUser model)
  {
    
      string uniqueFileName = null;
      if (model.ProfilePicture != null)
      {
        uniqueFileName = await _fileService.UploadFileAsync(model.ProfilePicture, "uploads");
      }
    Userslogin user = await _userRepository.GetUserByIdAsync(model.Userid);
    if (user == null)
    {
      return false;
    }
    else
    {

      user.Username = model.Username;
      user.User.Firstname = model.Firstname;
      user.User.Lastname = model.Lastname;
      user.User.Phone = model.Phone;
      user.User.Countryid = model.Countryid;
      user.User.Stateid = model.Stateid;
      user.User.Cityid = model.Cityid;
      user.User.Address = model.Address;
      user.User.Zipcode = model.Zipcode;
      user.Roleid = model.Roleid;
      user.status = model.status;
      user.Passwordhash = model.Password;
      user.Email= model.Email;
      user.User.Profileimg = uniqueFileName;

      await _userRepository.UpdateUserLoginAsync(user);
      return true;
    }
  }

  public async Task<bool> Delete(int id)
  {
    if (await _userDetailsRepository.Delete(id))
    {
      return true;
    }
    else
    {
      return false;
    }

  }

}
