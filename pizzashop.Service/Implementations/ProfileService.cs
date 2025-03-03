using Microsoft.AspNetCore.Mvc;
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

    public ProfileService(IUserRepository userRepository,IUserDetailsRepository userDetailsRepository)
    {
        _userRepository = userRepository;
        _userDetailsRepository = userDetailsRepository;
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
                Rolename=user.Role.Rolename
            };
            return viewmodel;
    }

    public async Task UpdateProfile(int id,UserProfile viewmodel)
    {

        User user = await _userDetailsRepository.GetUserByIdAsync(id);
        user.Firstname=viewmodel.Firstname;
        user.Lastname= viewmodel.Lastname;
        user.Phone=viewmodel.Phone;
        user.Zipcode= viewmodel.Zipcode;

        await _userDetailsRepository.UpdateAsync(user);
    }
}
