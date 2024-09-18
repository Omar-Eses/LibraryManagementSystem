using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Commands.BookCommandsHandlers;

public class CreateBookCommand : IRequest<Book>
{
    public string BookTitle { get; set; }
    public string BookDescription { get; set; }
    public string ISBN { get; set; }
    public string BookGenre { get; set; }
    public string BookPublisher { get; set; }
    public DateOnly BookPublishedDate { get; set; }
    public long AuthorId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class CreateBookCommandHandler(LMSContext context) : IRequestHandler<CreateBookCommand, Book>
{
    public async Task<Book> Handle(CreateBookCommand request)
    {
        var book = new Book
        {
            BookTitle = request.BookTitle,
            BookDescription = request.BookDescription,
            ISBN = request.ISBN,
            BookGenre = request.BookGenre,
            BookPublisher = request.BookPublisher,
            BookPublishedDate = request.BookPublishedDate,
            AuthorId = request.AuthorId,
            CreatedAt = request.CreatedAt.ToUniversalTime()
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        return book;
    }
}