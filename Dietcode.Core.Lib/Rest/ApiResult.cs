using System.Net;

namespace Dietcode.Core.Lib.Rest
{
    public class ApiResult<TResponse> where TResponse : class, new()
    {
        public TResponse Data { get; set; } = new();
        public HttpStatusCode StatusCode { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
