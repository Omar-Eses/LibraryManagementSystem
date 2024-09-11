using System.ComponentModel.DataAnnotations;
using static LibraryManagementSystem.Helpers.Enums;

namespace LibraryManagementSystem.Models;


public class Book
{
    public long Id { get; set; }
    [Required]
    public string BookTitle { get; set; }
    [Required]
    public string BookDescription { get; set; }
    [Required]
    public string ISBN { get; set; }
    [Required]
    public string BookGenre { get; set; }
    [Required]
    public string BookPublisher { get; set; }
    public DateOnly BookPublishedDate { get; set; }
    public long AuthorId { get; set; }
    public BorrowedStatus BorrowedStatus { get; set; }
    public long? borrowedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
