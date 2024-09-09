using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces;

public interface IAuthorsService
{
    Task<IEnumerable<Authors>> GetAllAuthorsAsync();
    Task<Authors?> GetAuthorByIdAsync(long id);
    Task<Authors> AddAuthorAsync(Authors author);
    Task<bool> UpdateAuthorAsync(long id, Authors author);
    Task<bool> DeleteAuthorAsync(long id);
    bool AuthorExists(long id);
}
