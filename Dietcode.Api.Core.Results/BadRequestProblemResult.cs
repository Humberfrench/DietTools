using Microsoft.AspNetCore.Mvc;

namespace Dietcode.Api.Core.Results
{
    public class BadRequestProblemResult : ErrorResult<ProblemDetails>
    {

        public BadRequestProblemResult(ErrorValidation error)
            : base(new ProblemDetails
            {
                Detail = error.Message,
                Title = "Bad Request",
                Status = 400,
            },
                    ResultStatusCode.BadRequest, error)
        {
        }

        public BadRequestProblemResult(ErrorValidation error, ProblemDetails content)
            : base(content, ResultStatusCode.BadRequest, error)
        {
        }

        public BadRequestProblemResult(IEnumerable<ErrorValidation> errors)
            : base(new ProblemDetails
            {
                Detail = string.Join('-', errors),
                Title = "Bad Request",
                Status = 400,
            },
                    ResultStatusCode.BadRequest, errors)
        {
        }
        public BadRequestProblemResult(IEnumerable<ErrorValidation> errors, ProblemDetails content)
            : base(content, ResultStatusCode.BadRequest, errors)
        {
        }
    }
}
