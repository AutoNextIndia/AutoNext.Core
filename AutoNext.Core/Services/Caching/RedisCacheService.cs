using AutoNext.Core.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace AutoNext.Core.Services.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _defaultOptions;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
            _defaultOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var data = await _cache.GetAsync(key, cancellationToken);

            if (data == null)
                return default;

            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions();

            if (expiry.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiry;
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = _defaultOptions.AbsoluteExpirationRelativeToNow;
                options.SlidingExpiration = _defaultOptions.SlidingExpiration;
            }

            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var data = Encoding.UTF8.GetBytes(json);
            await _cache.SetAsync(key, data, options, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            var data = await _cache.GetAsync(key, cancellationToken);
            return data != null;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            var cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue != null)
                return cachedValue;

            var value = await factory();
            await SetAsync(key, value, expiry, cancellationToken);

            return value;
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            // Note: Redis doesn't support prefix deletion natively
            // This is a simplified version. For production, consider using:
            // 1. Redis SCAN command with pattern matching
            // 2. Store keys in a set for batch deletion
            // 3. Use Redis KEYS command (not recommended for production)

            // For now, we'll log a warning
            Console.WriteLine($"Warning: RemoveByPrefixAsync for '{prefix}' not fully implemented in Redis");
            await Task.CompletedTask;
        }
    }
}
