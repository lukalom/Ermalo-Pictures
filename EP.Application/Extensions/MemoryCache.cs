using Microsoft.Extensions.Caching.Memory;

namespace EP.Application.Extensions
{
    public static class MemoryCache<T>
    {
        private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static T GetOrCreate(object key, Func<T>? createItem = null)
        {
            var cacheExpiryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(90),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(60)
            };

            if (createItem == null)
            {
                return (T)_cache.Get(key);
            }

            if (!_cache.TryGetValue(key, out T cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = createItem();

                _cache.Set(key, cacheEntry, cacheExpiryOptions);
            }

            return cacheEntry;
        }
    }
}
