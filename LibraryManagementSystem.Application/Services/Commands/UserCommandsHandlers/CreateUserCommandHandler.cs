using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Services.Commands.UserCommandsHandlers;

public class CreateUserCommand : IRequest<User>
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string HashedPassword { get; set; }
    public required string Phone { get; set; }
    public string? Address { get; set; }
    public List<long> UserPermissionsList { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IApplicationDbContext _context;

    public CreateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> Handle(CreateUserCommand request)
    {
        Console.WriteLine(request);
        if (string.IsNullOrEmpty(request.Username))
        {
            throw new ArgumentException("Username is required");
        }
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            HashedPassword = request.HashedPassword,
            Phone = request.Phone,
            Address = request.Address,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await AssignPermissionsToUser(user.Id, request.UserPermissionsList);
        return user;
    }

    private async Task AssignPermissionsToUser(long userId, List<long> permissionIds)
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
    }
}