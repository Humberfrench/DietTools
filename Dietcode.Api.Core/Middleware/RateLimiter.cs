using Microsoft.Extensions.Caching.Memory;

namespace Dietcode.Api.Core.Middleware
{
    public interface IRateLimiter
    {
        RateLimitResult Check(string key, int limit, TimeSpan window);
    }

    public class RateLimiter : IRateLimiter
    {
        private readonly IMemoryCache _cache;

        public RateLimiter(IMemoryCache cache)
        {
            _cache = cache;
        }

        private sealed class Counter
        {
            public int Count { get; set; }
            public DateTime WindowStart { get; set; }
        }

        public RateLimitResult Check(string key, int limit, TimeSpan window)
        {
            var counter = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = window;
                return new Counter
                {
                    Count = 0,
                    WindowStart = DateTime.UtcNow
                };
            });

            // fallback defensivo (não deveria acontecer)
            counter ??= new Counter
            {
                Count = 0,
                WindowStart = DateTime.UtcNow
            };
            counter.Count++;

            var elapsed = DateTime.UtcNow - counter.WindowStart;
            var retryAfter = window - elapsed;
            if (retryAfter < TimeSpan.Zero)
                retryAfter = TimeSpan.Zero;

            return new RateLimitResult
            {
                IsLimited = counter.Count > limit,
                Remaining = Math.Max(0, limit - counter.Count),
                RetryAfter = retryAfter
            };
        }
    }
}
