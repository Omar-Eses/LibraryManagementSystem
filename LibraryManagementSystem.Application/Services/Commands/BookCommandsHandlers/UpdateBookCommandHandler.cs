using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Services.Commands.BookCommandsHandlers;

public class UpdateBookCommand : IRequest<Book>
{

    public long Id { get;set; }
    public string BookTitle { get; set; }
    public string BookDescription { get; set; }
    public string ISBN { get; set; }
    public string BookGenre { get; set; }
    public string BookPublisher { get; set; }
    public DateOnly BookPublishedDate { get; set; }
    public long AuthorId { get; set; }
    public Enums.BorrowedStatus BorrowedStatus { get; set; }
}
public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Book>
{
    private readonly IApplicationDbContext _context;

    public UpdateBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Book> Handle(UpdateBookCommand request)
    {
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null) throw new Exception("Book not found");

        book.BookTitle = request.BookTitle;
        book.BookDescription = request.BookDescription;
        book.ISBN = request.ISBN;
        book.BookGenre = request.BookGenre;
        book.BookPublisher = request.BookPublisher;
        book.BookPublishedDate = request.BookPublishedDate;
        book.AuthorId = request.AuthorId;
        book.BorrowedStatus = request.BorrowedStatus;
        book.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return book;
    }
}
