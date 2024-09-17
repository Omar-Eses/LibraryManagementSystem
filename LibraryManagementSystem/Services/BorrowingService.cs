using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using static LibraryManagementSystem.Helpers.Enums;

namespace LibraryManagementSystem.Services;

public class BorrowingService : IBorrowingService
{
    private readonly LMSContext _context;

    public BorrowingService(LMSContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<BorrowingRecord>> GetAllRecordsAsync()
    {
        return await _context.BorrowingRecord.ToListAsync();
    }

    public async Task<IEnumerable<BorrowingRecord>> GetBorrwoedBooksAsync()
    {
        return await _context.BorrowingRecord
            .Where(br => br.ReturnedDate == null)
            .ToListAsync();
    }

    public async Task<BorrowingRecord?> GetRecordByBookIdAsync(long bookId)
    {
        return await _context.BorrowingRecord
            .Where(br => br.BookId == bookId).FirstOrDefaultAsync();
    }

    public async Task<bool> BorrowBookAsync(long bookId, long userId)
    {
        var borrowingRecord = await GetRecordByBookIdAsync(bookId);
        if (borrowingRecord != null && borrowingRecord.ReturnedDate == null) return false;
        var newRecord = new BorrowingRecord
        {
            BookId = bookId,
            UserId = userId,
            BorrowedDate = DateTimeOffset.Now,
            ReturnedDate = null,
            DueDate = DateTimeOffset.Now.AddDays(CommonVariables.DaysToReturn)
        };

        await _context.BorrowingRecord.AddAsync(newRecord);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReturnBookAsync(long bookId, long userId)
    {
        var borrowingRecord = await GetRecordByBookIdAsync(bookId);
        if (borrowingRecord != null && borrowingRecord.UserId == userId && borrowingRecord.ReturnedDate == null)
        {
            borrowingRecord.ReturnedDate = DateTimeOffset.Now;
            borrowingRecord.Fine =
                CalculateFine(borrowingRecord.DueDate, borrowingRecord.ReturnedDate ?? DateTimeOffset.Now);
            _context.BorrowingRecord.Update(borrowingRecord);
            await _context.SaveChangesAsync();
            return true;
        }
        // user already returned: specific error

        // book not borrowed by user: specific error
        return false;
    }

    public async Task<List<long>> GetAvailableBooksAsync() => await _context.BorrowingRecord
        .Where(br => br.ReturnedDate != null)
        .Select(br => br.BookId)
        .ToListAsync();

    public async Task<List<long>> GetBorrowedBooksAsync() => await _context.BorrowingRecord
        .Where(br => br.ReturnedDate == null)
        .Select(br => br.BookId)
        .ToListAsync();

    private static double CalculateFine(DateTimeOffset dueDate, DateTimeOffset returnedDate)
    {
        return returnedDate > dueDate
            ? dueDate.Subtract(returnedDate).TotalDays
            : 0;
    }
}