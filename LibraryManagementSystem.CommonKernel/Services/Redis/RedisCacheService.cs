using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace LibraryManagementSystem.CommonKernel.Services.Redis;

public class RedisCacheService(IDistributedCache redisCache) : IRedisCacheService
{
    private readonly TimeSpan cacheDuration = CommonVariables.CacheExpirationTime;
    public async Task<T> GetCacheDataAsync<T>(string key)
    {
        var cachedData = await redisCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cachedData)) return JsonConvert.DeserializeObject<T>(cachedData);
        
        return default(T);
    }

    public async Task SetCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null)
    {
        var serializedData = JsonConvert.SerializeObject(data);
        await redisCache.SetStringAsync(key, serializedData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime ?? cacheDuration
        });
    }

    public async Task UpdateCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null)
    {
        await RemoveCacheDataAsync(key);
        await SetCacheDataAsync(key, data, expirationTime ?? cacheDuration);
    }

    public async Task RemoveCacheDataAsync(string key) => await redisCache.RemoveAsync(key);
}
