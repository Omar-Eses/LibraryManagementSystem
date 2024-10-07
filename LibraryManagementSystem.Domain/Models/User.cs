using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Domain.Helpers;

namespace LibraryManagementSystem.Domain.Models;

public class User
{
    public long Id { get; init; }
    [Required] public string Username { get; set; }
    [Required][EmailAddress] public string Email { get; set; }
    [Required] public string HashedPassword { get; set; }
    [Required][StringLength(15)] public string Phone { get; set; }

    public string? Address { get; set; }

    public int NumberOfBooksAllowed { get; set; } = CommonVariables.NumberOfBooksAllowed;
    public List<Book> Books { get; set; } = [];
    public decimal? TotalFine { get; set; } = 0;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }

    public List<UserPermission> UserPermissions { get; set; } = [];
}