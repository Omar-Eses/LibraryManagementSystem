using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Users
    {
        public long Id { get; set; }
        [Required]
        [StringLength(100)]
        public  string Username { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        [Required] // Data Annotation
        [StringLength(15)]
        public string Phone { get; set; }
        [Required] 
        public string HashedPassword { get; set; }
        public int NumberOfBooksAllowed { get; set; } = Helpers.CommonVariables.NumberOfBooksAllowed;
        public List<Books> Books { get; set; } = new List<Books>();
        public decimal? TotalFine { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
