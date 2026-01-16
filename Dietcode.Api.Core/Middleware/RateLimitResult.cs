namespace Dietcode.Api.Core.Middleware
{
    public sealed class RateLimitResult
    {
        public bool IsLimited { get; init; }
        public int Remaining { get; init; }
        public TimeSpan RetryAfter { get; init; }
    }
}
