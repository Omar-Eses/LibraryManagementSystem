using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Queries;
public class GetBookQueryById : IRequest<Book>
{
    public long Id { get; set; }
}
public class GetBookQueryHandler : IRequestHandler<GetBookQueryById, Book>
{
    private readonly LMSContext _context;
    public GetBookQueryHandler(LMSContext context)
    {
        _context = context;
    }

    public async Task<Book> Handle(GetBookQueryById request)
    {
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null)
        {
            throw new Exception("Book not found");
        }

        return book;
    }
}
