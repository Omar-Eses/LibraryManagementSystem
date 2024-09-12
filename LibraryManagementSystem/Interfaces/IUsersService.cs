using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces;

public interface IUsersService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(long id);
    Task<User?> GetUserByEmailAsync(string email);
    
    Task<User> AddUserAsync(User user, List<long>? permissionIds);
    Task<bool> UpdateUserAsync(long id, User user, List<long>? permissionIds);
    Task<bool> DeleteUserAsync(long id);

    //TODO : check the user exists logic
    Task<bool> UserExistsAsync(long id);
    Task<bool> EmailExistsAsync(string email);
    Task<List<Permission>> GetUserPermissionsAsync(long userId);
}
