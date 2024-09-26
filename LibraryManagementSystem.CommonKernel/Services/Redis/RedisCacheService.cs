using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace LibraryManagementSystem.CommonKernel.Services.Redis;

public class RedisCacheService(IDistributedCache redisCache) : IRedisCacheService
{
    private readonly IDistributedCache _redisCache = redisCache;
    private readonly TimeSpan cacheTimeSpan = CommonVariables.CacheExpirationTime;
    public async Task<T> GetCacheDataAsync<T>(string key)
    {
        var cachedData = await _redisCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonConvert.DeserializeObject<T>(cachedData);
        }
        return default;
    }

    public async Task SetCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null)
    {
        var serializedData = JsonConvert.SerializeObject(data);
        await _redisCache.SetStringAsync(key, serializedData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime ?? cacheTimeSpan
        });
    }

    public async Task UpdateCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null)
    {

        await SetCacheDataAsync(key, data, expirationTime ?? cacheTimeSpan);
    }

    public async Task RemoveCacheDataAsync(string key) => await _redisCache.RemoveAsync(key);
}
