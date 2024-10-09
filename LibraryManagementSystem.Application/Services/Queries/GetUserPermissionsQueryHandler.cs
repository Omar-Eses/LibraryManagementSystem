using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Services.Queries;

public class GetUserPermissionsQuery : IRequest<List<Permission>>
{
    public long userId { get; set; }
}

public class GetUserPermissionsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetUserPermissionsQuery, List<Permission>>
{
    public async Task<List<Permission>> Handle(GetUserPermissionsQuery request)
    {
        var user = await context.Users.FindAsync(request.userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var userPermissions = await context.UserPermissions
            .Where(up => up.UserId == request.userId)
            .Include(up => up.Permission)
            .Select(up => up.Permission)
            .ToListAsync();

        return userPermissions;
    }
}