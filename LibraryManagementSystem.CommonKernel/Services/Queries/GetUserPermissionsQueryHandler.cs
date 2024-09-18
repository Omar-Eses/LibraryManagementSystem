using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetUserPermissionsQuery : IRequest<List<Permission>>
{
    public long userId { get; set; }
}

public class GetUserPermissionsQueryHandler(LMSContext context)
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
            .Include(up => up.Permission) // Include permission details
            //.AsNoTracking()
            //.AsSplitQuery()
            .Select(up => up.Permission) // Extract permission information
            .ToListAsync();

        return userPermissions;
    }
}