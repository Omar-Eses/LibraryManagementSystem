namespace LibraryManagementSystem.Application.Interfaces;

public interface IRedisCacheService
{
    Task<T> GetCacheDataAsync<T>(string key);
    Task SetCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null);
    Task UpdateCacheDataAsync<T>(string cacheKey, T data, TimeSpan? expirationTime = null);
    Task RemoveCacheDataAsync(string key);
}