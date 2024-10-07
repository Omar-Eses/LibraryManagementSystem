using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Services.Commands.BorrowingCommandsHandlers;

public class UpdateBorrowingCommand : IRequest<BorrowingRecord>
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset? ReturnedDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? DueDate { get; set; } = null;
}

public class UpdateBorrowingCommandHandler : IRequestHandler<UpdateBorrowingCommand, BorrowingRecord>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateBorrowingCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BorrowingRecord> Handle(UpdateBorrowingCommand request)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("User not authenticated");

            var userId = long.Parse(userIdClaim.Value);

            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null) throw new Exception("Book not found");

            var borrowingRecord = await _context.BorrowingRecords
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
            _context.BorrowingRecords.Update(borrowingRecord);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return borrowingRecord;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
