namespace LibraryManagementSystem.Models;

public class BorrowingRecord
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public long UserId { get; set; }
    public DateOnly BorrowedDate { get; set; }
    public DateOnly? ReturnedDate { get; set; }
    public DateOnly DueDate { get; set; } // DateTimeoffset
    public decimal Fine { get; internal set; } = 0;
}
