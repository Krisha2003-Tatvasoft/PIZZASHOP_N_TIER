using Microsoft.EntityFrameworkCore;
using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class UserDetailsRepository : IUserDetailsRepository
{
    private readonly PizzashopContext _context;

    public UserDetailsRepository(PizzashopContext context)
    {
        _context = context;
    }


    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Userid == id);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
