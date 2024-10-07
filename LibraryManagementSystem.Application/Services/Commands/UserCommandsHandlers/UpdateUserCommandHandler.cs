using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Services.Commands.UserCommandsHandlers;

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

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> Handle(UpdateUserCommand request)
    {
        var user = await _context.Users.FindAsync(request.Id);
        if (user == null) throw new Exception("User not found");
        user.Username = request.Username;
        user.HashedPassword = request.HashedPassword;
        user.NumberOfBooksAllowed = request.NumberOfBooksAllowed;
        user.TotalFine = request.TotalFine;
        user.UpdatedAt = request.UpdatedAt;
        await UpdatePermissionsToUser(request.Id, request.UserPermissionsList);
        await _context.SaveChangesAsync();

        return user;
    }

    private async Task<bool> UpdatePermissionsToUser(long userId, List<long> permissionIds)
    {
        var existingPermissions = _context.UserPermissions.Where(up => up.UserId == userId);
        _context.UserPermissions.RemoveRange(existingPermissions);
        foreach (var permissionId in permissionIds)
        {
            _context.UserPermissions.Add(new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }
}