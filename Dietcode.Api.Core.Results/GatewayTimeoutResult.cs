namespace Dietcode.Api.Core.Results
{
    public class GatewayTimeoutResult : ErrorResult
    {
        public GatewayTimeoutResult(ErrorValidation error)
            : base(ResultStatusCode.GatewayTimeout, error)
        {
        }

        public GatewayTimeoutResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.GatewayTimeout, errors)
        {
        }
    }
}
