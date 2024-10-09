using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IBooksService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(long id);
    Task<Book> AddBookAsync(Book book);
    Task<bool> UpdateBookAsync(long id, Book book);
    Task<bool> DeleteBookAsync(long id);
    bool BookExists(long id);
}
