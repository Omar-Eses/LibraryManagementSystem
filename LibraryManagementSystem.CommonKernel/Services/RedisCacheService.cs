using LibraryManagementSystem.CommonKernel.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace LibraryManagementSystem.CommonKernel.Services;

public class RedisCacheService(IDistributedCache redisCache) : IRedisCacheService
{
    private readonly IDistributedCache _redisCache = redisCache;

    public async Task<T> GetCacheDataAsync<T>(string key)
    {
        var cachedData = await _redisCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonConvert.DeserializeObject<T>(cachedData);
        }
        return default(T);
    }

    public async Task SetCacheDataAsync<T>(string key, T data, TimeSpan expirationTime)
    {
        var serializedData = JsonConvert.SerializeObject(data);
        await _redisCache.SetStringAsync(key, serializedData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        });
    }

    public async Task UpdateCacheDataAsync<T>(string key, T data, TimeSpan expirationTime)
    {
        await RemoveCacheDataAsync(key);
        await SetCacheDataAsync(key, data, expirationTime);
    }

    public async Task RemoveCacheDataAsync(string key) => await _redisCache.RemoveAsync(key);
}
