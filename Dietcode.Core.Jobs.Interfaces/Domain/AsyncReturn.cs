using Dietcode.Api.Core.Results;

namespace Dietcode.Core.Jobs.Interfaces.Domain
{
    public class AsyncReturn
    {
        public string IdempotencyKey { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public string StatusText => Status.ToString();
        public ResultStatusCode StatusCode { get; set; }
    }
}
