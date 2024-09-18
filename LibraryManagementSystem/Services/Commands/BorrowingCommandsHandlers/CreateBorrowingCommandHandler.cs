using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Commands.BorrowingCommandsHandlers;

public class CreateBorrowingCommand : IRequest<BorrowingRecord>
{
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset DueDate { get; set; } = DateTimeOffset.Now.AddDays(14);
    public double Fine { get; set; } = 0;
}

public class CreateBorrowingCommandHandler(LMSContext context)
    : IRequestHandler<CreateBorrowingCommand, BorrowingRecord>
{
    public async Task<BorrowingRecord?> Handle(CreateBorrowingCommand request)
    {
        try
        {
            var book = await context.Books.FindAsync(request.BookId);
            if (book == null) throw new Exception("Book not found");

            var user = await context.Users.FindAsync(request.UserId);
            if (user == null) throw new Exception("User not found");

            if (book.BorrowedStatus == Enums.BorrowedStatus.Borrowed) throw new Exception("Book is already borrowed");

            var borrowingRecord = new BorrowingRecord
            {
                BookId = request.BookId,
                UserId = request.UserId,
                DueDate = request.DueDate.ToUniversalTime(),
                Fine = request.Fine
            };
            book.BorrowedStatus = Enums.BorrowedStatus.Borrowed;
            context.BorrowingRecord.Add(borrowingRecord);
            context.Books.Update(book);
            await context.SaveChangesAsync();

            return borrowingRecord;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}