using pizzashop.Repository.Interfaces;
using pizzashop.Entity.Models;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using pizzashop.Entity.ViewModels;
using Org.BouncyCastle.Asn1.Mozilla;


namespace pizzashop.Service.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Userslogin?> AuthenticateUser(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null || !PasswordUtills.VerifyPassword(password, user.Passwordhash) || user.User.Isdeleted == true || user.status == Enums.statustype.Inactive)
            return null;

        return user;
    }

    public async Task<Role?> CheckRole(string role)
    {
        return await _roleRepository.GetRoleByNameAsync(role);
    }

    public async Task<bool>  UserExistsAsync(Forget viewmodel)
    {
      return await _userRepository.UserExistsAsync(viewmodel.Email);    
    }


   public async Task ResetPassword(ResetPassword model)
   {
        var user = await _userRepository.GetUserLoginByEmailAsync(model.Email);
        if(user!=null)
        {
            user.Passwordhash = PasswordUtills.HashPassword(model.Password);
            await _userRepository.UpdateUserLoginAsync(user);
        }
    }
    


}
