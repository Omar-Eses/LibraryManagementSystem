﻿using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Queries;

public class GetUserByIdQuery : IRequest<User>
{
    public long Id { get; set; }
}

public class GetUserQueryHandler(LMSContext context, IRedisCacheService cacheService) : IRequestHandler<GetUserByIdQuery, User>
{
    private readonly TimeSpan _cacheDuration = CommonVariables.CacheExpirationTime;
    public async Task<User> Handle(GetUserByIdQuery request)
    {
        var cachedUser = await cacheService.GetCacheDataAsync<User>($"User_{request.Id}");
        if (cachedUser != null) return cachedUser;

        var user = await context.Users.FindAsync(request.Id) ?? throw new Exception("User not found");

        CacheUserAsync(user);
        return user;
        // return await context.Users.FindAsync(request.Id) ?? throw new Exception("User not found");
    }

    private void CacheUserAsync(User user)
        => Task.Run(() => cacheService.SetCacheDataAsync($"User_{user.Id}", user, _cacheDuration));
}