using pizzashop.Repository.Interfaces;
using pizzashop.Repository.Models;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

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
        if (user == null || !PasswordUtills.VerifyPassword(password, user.Passwordhash))
            return null;

        return user;
    }

    public async Task<Role?> CheckRole(string role)
    {
        return await _roleRepository.GetRoleByNameAsync(role);
    }

}
