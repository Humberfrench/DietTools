using Dietcode.Api.Core.Results.Interfaces;

namespace Dietcode.Api.Core.Results
{
    public class OkResult : MethodResult
    {
        public OkResult() : base(ResultStatusCode.OK)
        {
        }
    }
    public class OkResult<T> : MethodResult<T>, IContentResult<T>
    {
        public OkResult(T content)
            : base(content, ResultStatusCode.OK)
        {
        }

        object IContentResult.Content => Content!;
    }

}
