using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Queries;
public class GetBookQueryById : IRequest<Book>
{
    public long Id { get; set; }
}
public class GetBookQueryHandler(LMSContext context) : IRequestHandler<GetBookQueryById, Book>
{
    public async Task<Book> Handle(GetBookQueryById request)
    {
        var book = await context.Books.FindAsync(request.Id);
        if (book == null)
        {
            throw new Exception("Book not found");
        }

        return book;
    }
}
