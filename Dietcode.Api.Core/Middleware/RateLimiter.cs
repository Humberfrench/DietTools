using Microsoft.Extensions.Caching.Memory;

namespace Dietcode.Api.Core.Middleware
{
    public interface IRateLimiter
    {
        bool IsLimited(string key, int limit, TimeSpan window);
    }

    public class RateLimiter(IMemoryCache cache) : IRateLimiter
    {
        private readonly IMemoryCache _cache = cache;

        public bool IsLimited(string key, int limit, TimeSpan window)
        {
            var count = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = window;
                return 0;
            });

            count++;

            _cache.Set(key, count, window);

            return count > limit;
        }
    }
}
