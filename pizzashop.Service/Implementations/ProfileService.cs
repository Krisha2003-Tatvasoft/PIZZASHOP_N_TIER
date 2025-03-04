using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;


namespace pizzashop.Service.Implementations;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDetailsRepository _userDetailsRepository;

    private readonly ICountryRepository _countryRepository;

     private readonly IStateRepository _stateRepository;
      private readonly ICityRepository _cityRepository;

    public ProfileService(IUserRepository userRepository, IUserDetailsRepository userDetailsRepository, 
    ICountryRepository countryRepository,IStateRepository stateRepository,ICityRepository cityRepository)
    {
        _userRepository = userRepository;
        _userDetailsRepository = userDetailsRepository;
        _countryRepository = countryRepository;
        _stateRepository = stateRepository;
        _cityRepository = cityRepository;
    }
    public async Task ChangePassword(string email, ChangePassword model)
    {
        var user = await _userRepository.GetUserLoginByEmailAsync(email);
        if (user == null || !PasswordUtills.VerifyPassword(model.OldPassword, user.Passwordhash))
        {
            return;
        }
        else
        {
            user.Passwordhash = PasswordUtills.HashPassword(model.Newpassword);
            await _userRepository.UpdateUserLoginAsync(user);
        }
    }


    public async Task<UserProfile> UserProfile(string email)
    {

        var user = await _userRepository.GetUserByEmailAsync(email);
        UserProfile viewmodel = new UserProfile
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
            Rolename = user.Role.Rolename,
            Countries=await _countryRepository.GetAllCountryAsync(),
            States  = await _stateRepository.GetStatesByCountryAsync(user.User.Countryid),
            Cities = await _cityRepository.GetCitiesByStateAsync(user.User.Stateid)
        };
        return viewmodel;
    }

    public async Task UpdateProfile(int id, UserProfile viewmodel)
    {

        User user = await _userDetailsRepository.GetUserByIdAsync(id);
        user.Firstname = viewmodel.Firstname;
        user.Lastname = viewmodel.Lastname;
        user.Phone = viewmodel.Phone;
        user.Zipcode = viewmodel.Zipcode;
        user.Countryid = viewmodel.Countryid;
        user.Stateid = viewmodel.Stateid;
        user.Cityid=viewmodel.Cityid;

        await _userDetailsRepository.UpdateAsync(user);
    }

   public async Task<List<SelectListItem>> GetStatesByCountryAsync(int countryId)
   {
      return await _stateRepository.GetStatesByCountryAsync(countryId);
   }

    public async Task<List<SelectListItem>> GetCitiesByStateAsync(int stateId)
    {
        return await _cityRepository.GetCitiesByStateAsync(stateId);
    }


}
