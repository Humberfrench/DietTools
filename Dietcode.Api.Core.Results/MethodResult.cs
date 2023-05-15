namespace Dietcode.Api.Core.Results
{
    public class MethodResult
    {
        public MethodResult(ResultStatusCode status)
        {
            Status = status;
        }

        public ResultStatusCode Status { get; set; }
    }



}
