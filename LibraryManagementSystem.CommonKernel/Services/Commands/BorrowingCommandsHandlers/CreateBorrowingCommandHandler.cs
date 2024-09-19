using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystem.Services.Commands.BorrowingCommandsHandlers;

public class CreateBorrowingCommand : IRequest<BorrowingRecord>
{
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset DueDate { get; set; } = DateTimeOffset.Now.AddDays(14);
    public double Fine { get; set; } = 0;
}

public class CreateBorrowingCommandHandler(LMSContext context, IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateBorrowingCommand, BorrowingRecord>
{
    public async Task<BorrowingRecord> Handle(CreateBorrowingCommand request)
    {
        try
        {
            var book = await context.Books.FindAsync(request.BookId);
            if (book == null) throw new Exception("Book not found");

            // Get the user id from the claims
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("User not authenticated");

            var userId = long.Parse(userIdClaim.Value);

            var user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found");

            if (book.BorrowedStatus == Enums.BorrowedStatus.Borrowed) throw new Exception("Book is already borrowed");

            var borrowingRecord = new BorrowingRecord
            {
                BookId = request.BookId,
                UserId = userId,
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
