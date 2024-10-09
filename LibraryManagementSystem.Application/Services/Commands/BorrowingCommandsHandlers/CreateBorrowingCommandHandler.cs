using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Services.Commands.BorrowingCommandsHandlers;

public class CreateBorrowingCommand : IRequest<BorrowingRecord>
{
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset DueDate { get; set; } = DateTimeOffset.Now.AddDays(14);
    public double Fine { get; set; } = 0;
}

public class CreateBorrowingCommandHandler : IRequestHandler<CreateBorrowingCommand, BorrowingRecord>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateBorrowingCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BorrowingRecord> Handle(CreateBorrowingCommand request)
    {
        try
        {
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null) throw new Exception("Book not found");

            // Get the user id from the claims
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("User not authenticated");

            var userId = long.Parse(userIdClaim.Value);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
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
            _context.BorrowingRecords.Add(borrowingRecord);
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