using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Commands.BorrowingCommandsHandlers;

public class UpdateBorrowingCommand : IRequest<BorrowingRecord>
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset? ReturnedDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? DueDate { get; set; } = null;
}

public class UpdateBorrowingCommandHandler(LMSContext context)
    : IRequestHandler<UpdateBorrowingCommand, BorrowingRecord>
{
    public async Task<BorrowingRecord> Handle(UpdateBorrowingCommand request)
    {
        // Find the book
        var book = await context.Books.FindAsync(request.BookId);
        if (book == null) throw new Exception("Book not found");
        // Find the borrowing record
        var borrowingRecord = await context.BorrowingRecord
            .Where(
                br => br.BookId == request.BookId && br.UserId == request.UserId && br.ReturnedDate == null
            )
            .FirstOrDefaultAsync();

        if (borrowingRecord == null)
            throw new Exception("No active borrowing record found for this book and user");

        // Update the borrowing record with new returned and due dates
        borrowingRecord.ReturnedDate = request.ReturnedDate?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
        borrowingRecord.DueDate = request.DueDate?.ToUniversalTime() ?? borrowingRecord.DueDate;

        // Mark the book as available
        book.BorrowedStatus = Enums.BorrowedStatus.Available;

        // Save the changes
        context.BorrowingRecord.Update(borrowingRecord);
        context.Books.Update(book);
        await context.SaveChangesAsync();

        return borrowingRecord;
    }
}