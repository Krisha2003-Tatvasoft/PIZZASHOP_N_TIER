using Microsoft.EntityFrameworkCore;
using pizzashop.Repository.Interfaces;
using pizzashop.Repository.Models;

namespace pizzashop.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly PizzashopContext _context;

    public UserRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<Userslogin?> GetUserByEmailAsync(string email)
    {
        return await _context.Userslogins.Include(u => u.Role).Include(u => u.User)
                                   .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _context.Userslogins.AnyAsync(u => u.Email == email);
    }
}
