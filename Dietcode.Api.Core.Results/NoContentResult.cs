namespace Dietcode.Api.Core.Results
{
    public class NoContentResult : MethodResult
    {
        public NoContentResult()
            : base(ResultStatusCode.NoContent)
        {
        }

    }
}
