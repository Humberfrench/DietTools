namespace Dietcode.Api.Core.Results
{
    public class AcceptedResult<TContent> : MethodResult<TContent>
    {
        public object Identifier { get; set; }

        public AcceptedResult(TContent content, object identifier)
            : base(content, ResultStatusCode.Accepted)
        {
            Identifier = identifier;
        }
    }
}
