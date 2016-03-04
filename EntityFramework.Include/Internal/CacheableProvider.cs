using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EntityFramework.Include.Internal
{
    internal class CacheableProvider<TCacheKey, TCacheValue>
    {
        protected static ConcurrentDictionary<TCacheKey, TCacheValue> Cache { get; } = new ConcurrentDictionary<TCacheKey, TCacheValue>();

        protected static bool ExistCache(TCacheKey key)
        {
            return Cache.ContainsKey(key);
        }

        protected static TCacheValue FromCache(TCacheKey key)
        {
            return Cache[key];
        }

        protected static void AddCache(TCacheKey key, TCacheValue value)
        {
            Cache.AddOrUpdate(key, value, (cacheKey, cacheValue) => value);
        }
    }
}
