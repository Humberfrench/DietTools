namespace Dietcode.Api.Core.Results
{
    public class CreatedResult<TContent> : MethodResult<TContent>
    {
        public object Identifier { get; set; }

        public CreatedResult(TContent content, object identifier)
            : base(content, ResultStatusCode.Created)
        {
            Identifier = identifier;
        }
    }
}
