namespace LibraryManagementSystem.Models;

public class BorrowingRecord
{
    public long Id { get; init; }
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset BorrowedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ReturnedDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public double Fine { get; internal set; } = 0;
}
