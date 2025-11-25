namespace Dietcode.Api.Core.Results
{
    public class InternalServerErrorResult : MethodResult
    {
        public Exception Exception { get; }

        public InternalServerErrorResult(Exception ex)
            : base(ResultStatusCode.InternalServerError)
        {
            Exception = ex;
        }
    }
}
