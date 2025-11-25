using Dietcode.Api.Core.Results.Interfaces;

namespace Dietcode.Api.Core.Results
{
    public class AcceptedResult : MethodResult
    {
        public AcceptedResult()
            : base(ResultStatusCode.Accepted)
        {
        }
    }
    public class AcceptedResult<T> : MethodResult<T>, IContentResult<T>
    {
        public object Identifier { get; }

        public AcceptedResult(T content, object identifier)
            : base(content, ResultStatusCode.Accepted)
        {
            Identifier = identifier;
        }

        object IContentResult.Content => Content!;
    }

}
