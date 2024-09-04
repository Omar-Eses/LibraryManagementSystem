namespace LibraryManagementSystem.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserHashedPassword { get; set; }
        public int numberOfBooksAllowed { get; set; }
        public List<Books> Books { get; set; }
        public float TotalFine { get; set; }
    }
}
