namespace Dietcode.Api.Core.Results
{
    public class OkResult<TContent> : MethodResult<TContent>
    {
        public OkResult(TContent content)
            : base(content, ResultStatusCode.OK)
        {
        }
    }
}
