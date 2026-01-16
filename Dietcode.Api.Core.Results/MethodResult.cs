namespace Dietcode.Api.Core.Results
{
    public class MethodResult
    {
        public MethodResult(ResultStatusCode status)
        {
            Status = status;
        }

        public bool IsError => (int)Status >= 400;

        public ResultStatusCode Status { get; set; }
    }



}
