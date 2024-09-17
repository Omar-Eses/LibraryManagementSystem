using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Queries;

public class GetBorrowingQueryById : IRequest<BorrowingRecord>
{
    public long Id { get; set; }
}
public class GetBorrowingQueryHandler : IRequestHandler<GetBorrowingQueryById, BorrowingRecord>
{
    private readonly LMSContext _context;
    public GetBorrowingQueryHandler(LMSContext context)
    {
        _context = context;
    }

    public async Task<BorrowingRecord> Handle(GetBorrowingQueryById request)
    {
        var borrowing = await _context.BorrowingRecord.FindAsync(request.Id);
        if (borrowing == null)
            throw new Exception("Borrowing not found");


        return borrowing;
    }
}
