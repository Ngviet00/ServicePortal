using Microsoft.Extensions.Caching.Memory;

namespace ServicePortal.Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            return _cache.TryGetValue(key, out var value) ? (T?)value : default;
        }

        public T GetOrCreate<T>(string key, Func<T> getData, int expireMinutes = 5)
        {
            if (!_cache.TryGetValue(key, out var cachedValue))
            {
                var value = getData();
                Set(key, value!, expireMinutes);
                return value;
            }

            return (T)cachedValue!;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getData, int expireMinutes = 5)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return (T)value!;
            }

            var result = await getData();
            Set(key, result!, expireMinutes);
            return result;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Set(string key, object value, int expireMinutes = 5)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expireMinutes)
            };

            _cache.Set(key, value, options);
        }
    }
}
