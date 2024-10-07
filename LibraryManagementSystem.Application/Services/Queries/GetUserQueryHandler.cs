
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Services.Queries;

public class GetUserByIdQuery : IRequest<User>
{
    public long Id { get; set; }
}

public class GetUserQueryHandler(IApplicationDbContext context, IRedisCacheService cacheService) : IRequestHandler<GetUserByIdQuery, User>
{

    public async Task<User> Handle(GetUserByIdQuery request)
    {
        var cachedUser = await cacheService.GetCacheDataAsync<User>($"User_{request.Id}");
        if (cachedUser != null) return cachedUser;

        var user = await context.Users.FindAsync(request.Id) ?? throw new Exception("User not found");
        await cacheService.SetCacheDataAsync($"User_{user.Id}", user);

        return user;
    }
}
