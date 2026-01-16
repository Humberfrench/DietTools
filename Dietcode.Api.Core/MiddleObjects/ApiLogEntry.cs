namespace Dietcode.Api.Core.MiddleObjects
{
    public sealed class ApiLogEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Method { get; set; } = default!;
        public string Url { get; set; } = default!;
        public object? Request { get; set; }
        public object? Response { get; set; }
        public int StatusCode { get; set; }
        public string TraceId { get; set; } = default!;
    }
}
