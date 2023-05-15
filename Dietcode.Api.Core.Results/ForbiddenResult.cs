namespace Dietcode.Api.Core.Results
{
    public class ForbiddenResult : MethodResult
    {
        public ForbiddenResult()
                : base(ResultStatusCode.Forbidden)
        {
        }
    }
}
