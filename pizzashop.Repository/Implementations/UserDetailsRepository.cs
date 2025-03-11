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

    public async Task AddUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var userById = await _context.Users.FirstOrDefaultAsync(s => s.Userid == id);
        if (userById != null)
        {
            userById.Isdeleted = true;
            _context.Users.Update(userById);
            await _context.SaveChangesAsync();

            return true;
        }
        else
        {
            return false;
        }

    }

    public async Task<bool> PhoneExistsAsync(string phone)
    {
        return await _context.Users.AnyAsync(u => u.Phone == phone);
    }

}
