namespace Dietcode.Api.Core.Results
{
    public class TimeOutResult : ErrorResult
    {
        public TimeOutResult(ErrorValidation error)
            : base(ResultStatusCode.TimeOut, error)
        {
        }

        public TimeOutResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.TimeOut, errors)
        {
        }
    }
}
