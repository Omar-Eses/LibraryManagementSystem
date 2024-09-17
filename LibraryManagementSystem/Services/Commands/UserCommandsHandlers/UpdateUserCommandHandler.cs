using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Commands.UserCommandsHandlers;

public class UpdateUserCommand : IRequest<User>
{
    public long Id { get; }
    public string HashedPassword { get; set; }
    public int NumberOfBooksAllowed { get; set; }
    public List<Book> books { get; set; }
    public decimal? TotalFine { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public List<long> UserPermissionsList { get; set; }
}
public class UpdateUserCommandHandler(LMSContext context) : IRequestHandler<UpdateUserCommand, User>
{
    public async Task<User> Handle(UpdateUserCommand request)
    {
        var user = new User
        {
            HashedPassword = request.HashedPassword,
            NumberOfBooksAllowed = request.NumberOfBooksAllowed,
            Books = request.books,
            TotalFine = request.TotalFine,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        context.Users.Update(user);
        await context.SaveChangesAsync();

        await UpdatePermissionsToUser(user.Id, request.UserPermissionsList);

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
        return await Task.FromResult(true);
    }
}
