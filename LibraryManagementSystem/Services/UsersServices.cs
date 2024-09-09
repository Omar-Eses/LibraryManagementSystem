using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class UsersServices : IUsersService
{
    private readonly LMSContext _context;
    public UsersServices(LMSContext context)
    {
        _context = context;
    }
    public async Task<Users> AddUserAsync(Users user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;

    }

    public async Task<IEnumerable<Users>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<Users?> GetUserByIdAsync(long id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> UpdateUserAsync(long id, Users user)
    {
        if (id != user.Id)
            return await Task.FromResult(false);
        try
        {
            var oldUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (oldUser == null)
                return await Task.FromResult(false);
            oldUser.Username = user.Username;
            oldUser.Email = user.Email;
            oldUser.HashedPassword = user.HashedPassword;
            oldUser.Books = user.Books;
            oldUser.TotalFine = user.TotalFine;
            oldUser.UpdatedAt = DateTimeOffset.Now;
            _context.Users.Update(oldUser);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        } 
        catch (Exception e)
        {
            if (!UserExists(id))
            {
                Console.WriteLine(e.Message);
                return await Task.FromResult(false);
            }
            throw;
        }
    }

    public bool UserExists(long id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
