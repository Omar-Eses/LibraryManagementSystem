using LibraryManagementSystem.Data;
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
    public async Task<BorrowingRecord> Handle(CreateBorrowingCommand request)
    {
        var book = await context.Books.FindAsync(request.BookId);
        if (book == null) throw new Exception("Book not found");

        var user = await context.Users.FindAsync(request.UserId);
        if (user == null) throw new Exception("User not found");


        var borrowingRecord = new BorrowingRecord
        {
            BookId = request.BookId,
            UserId = request.UserId,
            DueDate = request.DueDate,
            Fine = request.Fine
        };

        context.BorrowingRecord.Add(borrowingRecord);
        await context.SaveChangesAsync();

        return borrowingRecord;
    }
}
