using System;
using System.Runtime.Caching;

public static class RateLimiter
{
    private static MemoryCache cache = MemoryCache.Default;

    public static bool Check(string clientId, int limit)
    {
        string key = "rate_" + clientId;

        int count = cache[key] as int? ?? 0;

        if (count >= limit)
            return false;

        cache.Set(key, count + 1, DateTimeOffset.Now.AddMinutes(1));

        return true;
    }
}
