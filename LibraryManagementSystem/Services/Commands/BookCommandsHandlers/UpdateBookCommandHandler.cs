using System.Runtime.Serialization;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using static LibraryManagementSystem.Helpers.Enums;

namespace LibraryManagementSystem.Services.Commands.BookCommandsHandlers;

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
    public BorrowedStatus BorrowedStatus { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
public class UpdateBookCommandHandler(LMSContext context) : IRequestHandler<UpdateBookCommand, Book>
{
    public async Task<Book> Handle(UpdateBookCommand request)
    {
        var book = await context.Books.FindAsync(request.Id);
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

        await context.SaveChangesAsync();

        return book;
    }
}
