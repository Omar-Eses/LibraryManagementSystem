using LibraryManagementSystem.Helpers;

namespace LibraryManagementSystem.Models;

public class Permission
{
    public long Id { get; init; }
    public string PermissionName { get; set; }
    public List<UserPermissions> UserPermissions { get; set; }
}
