namespace Dietcode.Api.Core.Results
{
    public class ServiceUnavailableResult : MethodResult
    {
        public Exception Exception { get; set; }

        public ServiceUnavailableResult(Exception exception)
            : base(ResultStatusCode.ServiceUnavailable)
        {
            Exception = exception;
        }
    }
}
