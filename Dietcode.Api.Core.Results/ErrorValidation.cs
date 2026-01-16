namespace Dietcode.Api.Core.Results
{
    public class ErrorValidation
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public ErrorValidation()
        {
            Code = string.Empty;
            Message = string.Empty;
        }

        public ErrorValidation(string code, string message)
        {
            Code = code;
            Message = message;
        }
        public ErrorValidation(int code, string message)
        {
            Code = code.ToString();
            Message = message;
        }
        public ErrorValidation(ResultStatusCode code, string message)
        {
            Code = code.ToString();
            Message = message;
        }

    }
}
