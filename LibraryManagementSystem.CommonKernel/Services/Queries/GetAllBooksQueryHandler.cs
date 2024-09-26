using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetAllBooksQuery : IRequest<IEnumerable<Book>>
{
}

public class GetAllBooksQueryHandler(LMSContext context, IRedisCacheService cacheService) : IRequestHandler<GetAllBooksQuery, IEnumerable<Book>>
{
    public async Task<IEnumerable<Book>> Handle(GetAllBooksQuery query)
    {
        var cachedBooks = new List<Book>();
        var uncachedBooksIds = new List<long>();
        var allBookIds = await context.Books.Select(b => b.Id).ToListAsync();

        foreach (var bookId in allBookIds)
        {
            var cachedBook = await cacheService.GetCacheDataAsync<Book>($"Book_{bookId}");
            
            if (cachedBook != null) cachedBooks.Add(cachedBook);
            else uncachedBooksIds.Add(bookId);

        }
        if (!uncachedBooksIds.Any()) return cachedBooks;
        
        var uncachedBooks = await context.Books.Where(b => uncachedBooksIds.Contains(b.Id)).ToListAsync();

        foreach (var book in uncachedBooks)
        {
            await cacheService.SetCacheDataAsync($"Book_{book.Id}", book);
        }

        cachedBooks.AddRange(uncachedBooks);
        return cachedBooks;
    }
}