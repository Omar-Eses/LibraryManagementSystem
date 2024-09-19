using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystem.Services.Commands.BorrowingCommandsHandlers;

public class UpdateBorrowingCommand : IRequest<BorrowingRecord>
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset? ReturnedDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? DueDate { get; set; } = null;
}

public class UpdateBorrowingCommandHandler(LMSContext context, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateBorrowingCommand, BorrowingRecord>
{
    public async Task<BorrowingRecord> Handle(UpdateBorrowingCommand request)
    {
        try
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("User not authenticated");

            var userId = long.Parse(userIdClaim.Value);

            var book = await context.Books.FindAsync(request.BookId);
            if (book == null) throw new Exception("Book not found");

            var borrowingRecord = await context.BorrowingRecord
                .Where(br => br.BookId == request.BookId && br.UserId == userId && br.ReturnedDate == null)
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
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
