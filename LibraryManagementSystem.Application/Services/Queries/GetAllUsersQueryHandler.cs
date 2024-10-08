﻿using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Services.Queries;

public class GetAllUsersQuery : IRequest<IEnumerable<User>>
{
}

public class GetAllUsersQueryHandler(IApplicationDbContext context, IRedisCacheService cacheService)
    : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
{
    private readonly TimeSpan _cacheDuration = CommonVariables.CacheExpirationTime;

    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query)
    {
        var cachedUsers = new List<User>();
        var uncachedUsersIds = new List<long>();
        var allUsersIds = await context.Users.Select(u => u.Id).ToListAsync();

        foreach (var userId in allUsersIds)
        {
            var cachedUser = await cacheService.GetCacheDataAsync<User>($"User_{userId}");
            if (cachedUser != null) cachedUsers.Add(cachedUser);
            else uncachedUsersIds.Add(userId);
        }

        if (!uncachedUsersIds.Any()) return cachedUsers;

        var uncachedUsers = await context.Users.Where(u => uncachedUsersIds.Contains(u.Id)).ToListAsync();
        foreach (var user in uncachedUsers)
        {
            await cacheService.SetCacheDataAsync($"User_{user.Id}", user);
        }

        cachedUsers.AddRange(uncachedUsers);
        return cachedUsers;
    }
}