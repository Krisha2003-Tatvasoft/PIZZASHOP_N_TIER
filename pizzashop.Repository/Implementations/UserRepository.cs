using Microsoft.EntityFrameworkCore;
using pizzashop.Repository.Interfaces;
using pizzashop.Entity.Models;

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

    public async Task<bool> UsernameExistsAsync(string Username)
    {
        return await _context.Userslogins.AnyAsync(u => u.Username == Username);
    }

     public async Task<bool> UsernameExistsEditAsync(string Username,int userid)
    {
        return await _context.Userslogins.AnyAsync(u => u.Username == Username && u.Userid!=userid);
    }


    public async Task<Userslogin?> GetUserLoginByEmailAsync(string email)
    {
        return await _context.Userslogins.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task UpdateUserLoginAsync(Userslogin userslogin)
    {
        _context.Userslogins.Update(userslogin);
        await _context.SaveChangesAsync();
    }
    public async Task<IQueryable<Userslogin>> GetFilteredAsync(string search, string SortColumn, string SortOrder)
    {
        string lowerSearch = search.ToLower();
        var userList = _context.Userslogins
        .Include(u => u.User)
        .Include(u => u.Role)
        .Where(u => u.User.Isdeleted == false)
        .Where(u => string.IsNullOrEmpty(lowerSearch) ||
                            u.User.Firstname.ToLower().Contains(lowerSearch) ||
                            u.User.Lastname.ToLower().Contains(lowerSearch) ||
                            u.Email.ToLower().Contains(lowerSearch) ||
                            u.User.Phone.ToLower().Contains(lowerSearch) ||
                            u.Role.Rolename.ToLower().Contains(lowerSearch));

        userList = SortColumn switch
        {
            "Firstname" => SortOrder == "desc" ? userList.OrderByDescending(u => u.User.Firstname) : userList.OrderBy(u => u.User.Firstname),
            "Rolename" => SortOrder == "desc" ? userList.OrderByDescending(u => u.Role.Rolename) : userList.OrderBy(u => u.Role.Rolename),
            // Add additional cases for other columns as needed
            _ => userList.OrderBy(u => u.User.Firstname),// Default sort (if none provided)
        };
        return userList;
    }

    public async Task AddNewUser(Userslogin newUserLogin)
    {
        _context.Userslogins.Add(newUserLogin);
        await _context.SaveChangesAsync();
    }

    public async Task<Userslogin?> GetUserByIdAsync(int id)
    {
        return await _context.Userslogins.Include(u => u.Role).Include(u => u.User)
                                   .FirstOrDefaultAsync(u => u.Userid == id);
    }


}
