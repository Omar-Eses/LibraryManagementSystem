using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Commands.BorrowingCommandsHandlers;


public class UpdateBorrowingCommand : IRequest<BorrowingRecord>
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset? ReturnedDate { get; set; } = DateTime.Now;
    public DateTimeOffset? DueDate { get; set; } = null;
}

public class UpdateBorrowingCommandHandler : IRequestHandler<UpdateBorrowingCommand, BorrowingRecord>
{
    private readonly LMSContext _context;
    public UpdateBorrowingCommandHandler(LMSContext context)
    {
        _context = context;
    }

    public async Task<BorrowingRecord> Handle(UpdateBorrowingCommand request)
    {
        var borrowingRecord = await _context.BorrowingRecord.FindAsync(request.Id);
        if (borrowingRecord == null) throw new Exception("Borrowing record not found");

        borrowingRecord.BookId = request.BookId;
        borrowingRecord.UserId = request.UserId;
        borrowingRecord.ReturnedDate = request.ReturnedDate;
        borrowingRecord.DueDate = request.DueDate ?? borrowingRecord.DueDate;

        await _context.SaveChangesAsync();
        
        return borrowingRecord;
    }
}
