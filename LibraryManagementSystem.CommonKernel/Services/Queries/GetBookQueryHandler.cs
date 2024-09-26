using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Queries;
public class GetBookQueryById : IRequest<Book>
{
    public long Id { get; set; }
}
public class GetBookQueryHandler(LMSContext context, IRedisCacheService cacheService) : IRequestHandler<GetBookQueryById, Book>
{
    private readonly TimeSpan _cacheDuration = CommonVariables.CacheExpirationTime;
    public async Task<Book> Handle(GetBookQueryById request)
    {
        // step 1 => get cached book if available
        var cachedBook = await cacheService.GetCacheDataAsync<Book>($"Book_{request.Id}");
        if (cachedBook != null) return cachedBook;
        // step 2 => else get book from DB
        var book = await context.Books.FindAsync(request.Id) ?? throw new Exception("Book not found");
        // step 3 => set cache in parallel call cachebookasync
        await cacheService.SetCacheDataAsync($"Book_{book.Id}", book);
        // step 4 => return book
        return book;
    }

}
