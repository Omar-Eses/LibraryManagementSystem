namespace LibraryManagementSystem.Models
{
    public class UserPermissions
    {
        public long Id { get; init; }
        public long UserId { get; set; }
        public User User { get; set; }
        public long PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
