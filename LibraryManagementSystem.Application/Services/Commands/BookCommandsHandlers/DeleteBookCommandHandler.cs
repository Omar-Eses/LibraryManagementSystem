using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Services.Commands.BookCommandsHandlers;

public class DeleteBookCommand : IRequest<Book>
{
    public long BookId { get; set; }
}

public class DeleteBookCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteBookCommand, Book>
{
    public async Task<Book> Handle(DeleteBookCommand request)
    {
        var book = await context.Books.FindAsync(request.BookId);
        if (book == null) throw new Exception("User not found");

        context.Books.Remove(book);
        await context.SaveChangesAsync();
        return book;
    }
}