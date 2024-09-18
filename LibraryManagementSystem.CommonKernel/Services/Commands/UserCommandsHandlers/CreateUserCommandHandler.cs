using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Commands.UserCommandsHandlers;

public class CreateUserCommand : IRequest<User>
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public string Phone { get; set; }
    public string? Address { get; set; }
    public List<long> UserPermissionsList { get; set; }
}

public class CreateUserCommandHandler(LMSContext context) : IRequestHandler<CreateUserCommand, User>
{
    public async Task<User> Handle(CreateUserCommand request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            HashedPassword = request.HashedPassword,
            Phone = request.Phone,
            Address = request.Address,
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        await AssignPermissionsToUser(user.Id, request.UserPermissionsList);
        return user;
    }

    private async Task AssignPermissionsToUser(long userId, List<long> permissionIds)
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
    }
}