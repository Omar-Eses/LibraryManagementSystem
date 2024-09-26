using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.CommonKernel.Services;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetUserByEmailQuery : IRequest<User>
{
    public string Email { get; set; }
}

public class GetUserByEmailQueryHandler(LMSContext context, IRedisCacheService redisCacheService) : IRequestHandler<GetUserByEmailQuery, User>
{
    public async Task<User> Handle(GetUserByEmailQuery request)
    {
        var cachedUser = await redisCacheService.GetCacheDataAsync<User>($"UserEmail_{request.Email}");
        if (cachedUser != null) return cachedUser;

        // step 2 user not found in cache so check in database
        var user = await context.Users.FirstOrDefaultAsync(user => user.Email == request.Email);
        if (user != null) await CacheUserByIdAndEmail(user);
        // step 3 user found in database so add to cache 
        return user;
    }

    private async Task CacheUserByIdAndEmail(User user)
    {
        await redisCacheService.SetCacheDataAsync($"UserEmail_{user.Email}", user);
        await redisCacheService.SetCacheDataAsync($"UserId_{user.Id}", user);
    }
}