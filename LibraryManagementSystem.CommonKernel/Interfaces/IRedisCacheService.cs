namespace LibraryManagementSystem.CommonKernel.Interfaces;

public interface IRedisCacheService
{
    Task<T> GetCacheDataAsync<T>(string key);
    Task SetCacheDataAsync<T>(string key, T data, TimeSpan expirationTime);
    Task UpdateCacheDataAsync<T>(string cacheKey, T data, TimeSpan expirationTime);
    Task RemoveCacheDataAsync(string key);
}