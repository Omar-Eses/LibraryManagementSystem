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
    public async Task<User> AddUserAsync(User user, List<long>? permissionIds)
    {

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        if (permissionIds != null)
            await AssignPermissionsToUser(user.Id, permissionIds);
        else
            // assign default permissions
            await AssignPermissionsToUser(user.Id, new List<long> { 1, 2, 3, 4, 5, 6 });
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

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UpdateUserAsync(long id, User user, List<long>? permissionIds)
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
            if (permissionIds != null)
                await AssignPermissionsToUser(user.Id, permissionIds);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            if (!await UserExistsAsync(id))
            {
                Console.WriteLine(e.Message);
                return await Task.FromResult(false);
            }
            throw;
        }
    }

    public async Task<bool> UserExistsAsync(long id)
        => await _context.Users.FindAsync(id) != null;

    public async Task<bool> EmailExistsAsync(string email)
        => await _context.Users.AnyAsync(u => u.Email == email);

    public async Task AssignPermissionsToUser(long userId, List<long>? permissionIds)
    {
        // check if user id is valid
        var user = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!user)
            throw new Exception("User not found");

        // remove existing permissions
        var existingPermissions = _context.UserPermissions.Where(up => up.UserId == userId);
        _context.UserPermissions.RemoveRange(existingPermissions);
        if (permissionIds == null)
        {
            await _context.SaveChangesAsync();
            return;
        }
        // Assign new permissions
        foreach (long permissionId in permissionIds)
        {
            _context.UserPermissions.Add(new UserPermissions
            {
                UserId = userId,
                PermissionId = permissionId
            });
        }

        // Save changes to the database
        await _context.SaveChangesAsync();

    }

    public async Task<List<Permission>> GetUserPermissionsAsync(long userId)
    {
        return await Task.FromResult(await _context.UserPermissions
            .Where(up => up.UserId == userId)
            .Include(up => up.Permission)
            .Select(up => up.Permission)
            .ToListAsync());
    }
}
