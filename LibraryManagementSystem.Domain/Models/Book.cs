using LibraryManagementSystem.Domain.Helpers;

namespace LibraryManagementSystem.Domain.Models;

public class Book
{
    public long Id { get; init; }
    public string BookTitle { get; set; } = null!;
    public string? BookDescription { get; set; }
    public string ISBN { get; set; }
    public string BookGenre { get; set; }
    public string BookPublisher { get; set; }
    public DateOnly BookPublishedDate { get; set; }
    public long AuthorId { get; set; }
    public Enums.BorrowedStatus BorrowedStatus { get; set; }
    public long? borrowedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
