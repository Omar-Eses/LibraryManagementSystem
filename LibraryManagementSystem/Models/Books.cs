namespace LibraryManagementSystem.Models;

public enum BorrowedStatus
{
    Borrowed = 1,
    Returned = 2
}
public class Books
{
    public int Id { get; set; }
    public string BookTitle { get; set; }
    public string BookDescription { get; set; }
    public string ISBN { get; set; }
    public string BookGenre { get; set; }
    public string BookPublisher { get; set; }
    public DateOnly BookPublishedDate { get; set; }
    public long AuthorId { get; set; }
    public string AuthorName { get; set; }
    public BorrowedStatus BorrowedStatus { get; set; }
    public long borrowedByUserId { get; set; }
    public string borrowedByUserName { get; set; }
}
