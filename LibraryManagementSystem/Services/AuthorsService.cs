
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class AuthorsService : IAuthorsService
{
    private readonly LMSContext _context;
    public AuthorsService(LMSContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Authors>> GetAllAuthorsAsync()
    {
        return await _context.Authors.ToListAsync();
    }

    public async Task<Authors?> GetAuthorByIdAsync(long id) => await _context.Authors.Where(a => a.Id == id).FirstOrDefaultAsync();

    public async Task<Authors> AddAuthorAsync(Authors author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task<bool> UpdateAuthorAsync(long id, Authors author)
    {
        if (id != author.Id)
            return await Task.FromResult(false);
        try
        {
            var oldAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (oldAuthor == null)
                return await Task.FromResult(false);
            oldAuthor.AuthorName = author.AuthorName;
            oldAuthor.Address = author.Address;
            oldAuthor.Phone = author.Phone;
            oldAuthor.Email = author.Email;
            oldAuthor.HashedPassword = author.HashedPassword;
            oldAuthor.Books = author.Books;
            oldAuthor.UpdatedAt = DateTimeOffset.Now;
            _context.Authors.Update(oldAuthor);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            if (!AuthorExists(id))
            {
                Console.WriteLine(e.Message);
                return await Task.FromResult(false);
            }
            throw;
        }

    }
    public async Task<bool> DeleteAuthorAsync(long id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null)
        {
            return false;
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return true;
    }
    public bool AuthorExists(long id)
    {
        return _context.Authors.Any(e => e.Id == id);
    }
}
