using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces;

public interface IBorrowingService
{
    Task<IEnumerable<BorrowingRecord>> GetAllRecordsAsync();
    Task<IEnumerable<BorrowingRecord>> GetBorrwoedBooksAsync();
    Task<BorrowingRecord?> GetRecordByBookIdAsync(long bookId);
    Task<bool> BorrowBookAsync(long bookId, long userId);
    Task<bool> ReturnBookAsync(long bookId, long userId);
    Task<List<long>> GetAvailableBooksAsync();
    Task<List<long>> GetBorrowedBooksAsync();
}
