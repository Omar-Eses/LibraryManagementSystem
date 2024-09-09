using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces;

public interface IUsersService
{
    Task<IEnumerable<Users>> GetAllUsersAsync();
    Task<Users?> GetUserByIdAsync(long id);
    Task<Users> AddUserAsync(Users user);
    Task<bool> UpdateUserAsync(long id, Users user);
    Task<bool> DeleteUserAsync(long id);
    bool UserExists(long id);
}
