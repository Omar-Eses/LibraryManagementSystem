namespace LibraryManagementSystem.Models
{
    public class Authors
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public List<Books> Books { get; set; } = new List<Books>();
    }
}
