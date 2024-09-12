using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class BooksServices : IBooksService
{
    private readonly LMSContext _context;
    public BooksServices(LMSContext context)
    {
        _context = context;
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }
    public bool BookExists(long id) => _context.Books.Any(e => e.Id == id);
    public async Task<bool> DeleteBookAsync(long id)
    {
        var oldBook = await _context.Books.FindAsync(id);
        if (oldBook == null) return false;

        _context.Books.Remove(oldBook);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<Book>> GetAllBooksAsync() => await _context.Books.ToListAsync();
    public async Task<Book?> GetBookByIdAsync(long id) => await _context.Books.FindAsync(id);
    public async Task<bool> UpdateBookAsync(long id, Book book)
    {
        if (id != book.Id) return await Task.FromResult(false);
        try
        {
            var oldBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (oldBook == null)
                return await Task.FromResult(false);
            oldBook.BookTitle = book.BookTitle;
            oldBook.BookDescription = book.BookDescription;
            oldBook.ISBN = book.ISBN;
            oldBook.BookGenre = book.BookGenre;
            oldBook.BookPublisher = book.BookPublisher;
            oldBook.BookPublishedDate = book.BookPublishedDate;
            oldBook.BorrowedStatus = book.BorrowedStatus;
            oldBook.UpdatedAt = DateTimeOffset.Now;
            _context.Books.Update(oldBook);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);

        }
        catch (Exception e)
        {
            if (!BookExists(id))
            {
                Console.WriteLine(e.Message);
                return await Task.FromResult(false);
            }
            throw;
        }
    }
}
