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
            private int _count;

            public DateTime WindowStart { get; init; }

            public int Increment()
            {
                return Interlocked.Increment(ref _count);
            }
        }

        public RateLimitResult Check(string key, int limit, TimeSpan window)
        {
            // Sem lock global de propósito: essa checagem existe para segurar flood/DDoS,
            // então ela precisa ser barata mesmo sob volume alto. Um lock único serializaria
            // TODA checagem de rate limit (de qualquer rota/IP), virando o próprio gargalo
            // durante um ataque (self-inflicted DoS na sua própria proteção). O IMemoryCache
            // não garante que o factory rode uma única vez por chave em corrida concorrente,
            // mas o pior caso é raro (só na criação a frio da janela) e de baixo impacto:
            // deixa passar 1-2 requisições a mais naquele instante, aceitável para um
            // limitador best-effort.
            var counter = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = window;
                return new Counter
                {
                    WindowStart = DateTime.UtcNow
                };
            })!;

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
