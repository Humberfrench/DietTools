namespace Dietcode.Api.Core.Results
{
    public class ServiceUnavailableResult : MethodResult
    {
        public ServiceUnavailableResult()
            : base(ResultStatusCode.ServiceUnavailable)
        {
        }
    }
}
