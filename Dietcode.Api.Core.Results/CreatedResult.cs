using Dietcode.Api.Core.Results.Interfaces;

namespace Dietcode.Api.Core.Results
{
    public class CreatedResult : MethodResult
    {
        public CreatedResult()
            : base(ResultStatusCode.Created)
        {
        }
    }
    public class CreatedResult<T> : MethodResult<T>, IContentResult<T>
    {
        public object Identifier { get; }

        public CreatedResult(T content, object identifier)
            : base(content, ResultStatusCode.Created)
        {
            Identifier = identifier;
        }

        object IContentResult.Content => Content!;
    }

}
