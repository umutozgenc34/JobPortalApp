using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace JobPortalApp.Shared.Caching;

public sealed class DistributedCacheService(IDistributedCache distributedCache, ILogger<DistributedCacheService> logger) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cached = await distributedCache.GetStringAsync(key);
            return cached is null ? default : JsonSerializer.Deserialize<T>(cached);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Cache read failed for key {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        try
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
            await distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Cache write failed for key {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(params string[] keys)
    {
        foreach (var key in keys)
        {
            try
            {
                await distributedCache.RemoveAsync(key);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Cache remove failed for key {CacheKey}", key);
            }
        }
    }
}
