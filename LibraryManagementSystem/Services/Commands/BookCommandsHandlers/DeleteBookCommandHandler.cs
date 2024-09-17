using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Commands.BookCommandsHandlers;

public class DeleteBookCommand : IRequest<Book>
{
    public long BookId { get; set; }
}
public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Book>
{
    private readonly LMSContext _context;
    public DeleteBookCommandHandler(LMSContext context)
    {
        _context = context;
    }

    public async Task<Book> Handle(DeleteBookCommand request)
    {
        var book = await _context.Books.FindAsync(request.BookId);
        if (book == null) throw new Exception("User not found");

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return book;
    }
}
