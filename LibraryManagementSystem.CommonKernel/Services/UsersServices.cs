using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class UsersServices(LMSContext context) : IUsersService
{
    public async Task<User> AddUserAsync(User user, List<long>? permissionIds)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        if (permissionIds != null)
            await AssignPermissionsToUser(user.Id, permissionIds);
        else
            await AssignPermissionsToUser(user.Id, CommonVariables.DefaultPermissions);

        return user;
    }

    public async Task<bool> DeleteUserAsync(long id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
            return false;
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UpdateUserAsync(long id, User user, List<long>? permissionIds)
    {
        if (id != user.Id)
            return await Task.FromResult(false);
        try
        {
            var oldUser = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (oldUser == null)
                return await Task.FromResult(false);
            oldUser.Username = user.Username;
            oldUser.Email = user.Email;
            oldUser.HashedPassword = user.HashedPassword;
            oldUser.Books = user.Books;
            oldUser.TotalFine = user.TotalFine;
            oldUser.UpdatedAt = DateTimeOffset.Now;
            context.Users.Update(oldUser);
            await context.SaveChangesAsync();
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
        => await context.Users.FindAsync(id) != null;

    public async Task<bool> EmailExistsAsync(string email)
        => await context.Users.AnyAsync(u => u.Email == email);

    private async Task AssignPermissionsToUser(long userId, List<long>? permissionIds)
    {
        // check if user id is valid
        var user = await context.Users.AnyAsync(u => u.Id == userId);
        if (!user)
            throw new Exception("User not found");

        // remove existing permissions
        var existingPermissions = context.UserPermissions.Where(up => up.UserId == userId);
        context.UserPermissions.RemoveRange(existingPermissions);
        if (permissionIds == null)
        {
            await context.SaveChangesAsync();
            return;
        }

        // Assign new permissions
        foreach (var permissionId in permissionIds)
        {
            context.UserPermissions.Add(new UserPermissions
            {
                UserId = userId,
                PermissionId = permissionId
            });
        }

        // Save changes to the database
        await context.SaveChangesAsync();
    }

    public async Task<List<Permission>> GetUserPermissionsAsync(long userId)
    {
        return await Task.FromResult(await context.UserPermissions
            .Where(up => up.UserId == userId)
            .Include(up => up.Permission)
            .Select(up => up.Permission)
            .ToListAsync());
    }
}