using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Authors
    {
        public long Id { get; set; }
        [Required] // Data Annotation
        [StringLength(100)]
        public string AuthorName { get; set; }
        public string? Address { get; set; }
        [Required] // Data Annotation
        [StringLength(15)]
        public string Phone { get; set; }
        [Required] // Data Annotation
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        [Required] // Data Annotation
        public string HashedPassword { get; set; }
        public List<Books> Books { get; set; } = new List<Books>();
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; } = null;
    }
}
