using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces
{
    public interface IPermissionsServices
    {
        Task<string> GenerateJwtToken(User user);
    }
}
