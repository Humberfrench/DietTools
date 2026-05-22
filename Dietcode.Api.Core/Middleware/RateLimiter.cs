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
        private readonly object _counterCreationLock = new();

        public RateLimiter(IMemoryCache cache)
        {
            _cache = cache;
        }

        private sealed class Counter
        {
            private int _count;

            public DateTime WindowStart { get; init; }

            public int Increment()
            {
                return Interlocked.Increment(ref _count);
            }
        }

        public RateLimitResult Check(string key, int limit, TimeSpan window)
        {
            Counter counter;

            lock (_counterCreationLock)
            {
                counter = _cache.GetOrCreate(key, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = window;
                    return new Counter
                    {
                        WindowStart = DateTime.UtcNow
                    };
                })!;
            }

            var currentCount = counter.Increment();

            var elapsed = DateTime.UtcNow - counter.WindowStart;
            var retryAfter = window - elapsed;
            if (retryAfter < TimeSpan.Zero)
                retryAfter = TimeSpan.Zero;

            return new RateLimitResult
            {
                IsLimited = currentCount > limit,
                Remaining = Math.Max(0, limit - currentCount),
                RetryAfter = retryAfter
            };
        }
    }
}
