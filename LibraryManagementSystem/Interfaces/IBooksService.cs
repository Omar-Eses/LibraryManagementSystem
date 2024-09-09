using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces;

public interface IBooksService
{
    Task<IEnumerable<Books>> GetAllBooksAsync();
    Task<Books?> GetBookByIdAsync(long id);
    Task<Books> AddBookAsync(Books book);
    Task<bool> UpdateBookAsync(long id, Books book);
    Task<bool> DeleteBookAsync(long id);
    bool BookExists(long id);
}
