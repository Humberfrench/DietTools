using System.Net;

namespace Dietcode.Core.Lib.Rest
{
    public class ApiResult<TResponse> where TResponse : class, new()
    {
        public TResponse Data { get; set; } = new();

        public HttpStatusCode StatusCode { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        // NOVO
        public bool IsSuccess { get; set; }

        // NOVO (raw body)
        public string Content { get; set; } = string.Empty;

        // Opcional (debug)
        public string? ContentType { get; set; }
        public long? ContentLength { get; set; }

        public string Error { get; set; } = string.Empty;
    }
}
