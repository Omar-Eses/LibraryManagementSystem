using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using UserPermissions = LibraryManagementSystem.Models.UserPermissions;

namespace LibraryManagementSystem.Services.Commands.UserCommandsHandlers;

public class UpdateUserCommand : IRequest<User>
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public int NumberOfBooksAllowed { get; set; }
    public decimal? TotalFine { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<long> UserPermissionsList { get; set; }
}

public class UpdateUserCommandHandler(LMSContext context) : IRequestHandler<UpdateUserCommand, User>
{
    public async Task<User> Handle(UpdateUserCommand request)
    {
        var user = await context.Users.FindAsync(request.Id);
        if (user == null) throw new Exception("User not found");
        user.Username = request.Username;
        user.HashedPassword = request.HashedPassword;
        user.NumberOfBooksAllowed = request.NumberOfBooksAllowed;
        user.TotalFine = request.TotalFine;
        user.UpdatedAt = request.UpdatedAt;
        await UpdatePermissionsToUser(request.Id, request.UserPermissionsList);
        await context.SaveChangesAsync();

        return user;
    }

    private async Task<bool> UpdatePermissionsToUser(long userId, List<long> permissionIds)
    {
        var existingPermissions = context.UserPermissions.Where(up => up.UserId == userId);
        context.UserPermissions.RemoveRange(existingPermissions);
        foreach (var permissionId in permissionIds)
        {
            context.UserPermissions.Add(new UserPermissions
            {
                UserId = userId,
                PermissionId = permissionId
            });
        }

        await context.SaveChangesAsync();
        return true;
    }
}