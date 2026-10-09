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
            : this(errors.ToList())
        {
        }

        // Materializa a lista uma única vez: o Detail usa as mensagens (não o ToString
        // do ErrorValidation) e Errors mantém os mesmos itens mesmo se o chamador
        // passou um enumerável de uso único.
        private BadRequestProblemResult(List<ErrorValidation> errors)
            : base(new ProblemDetails
            {
                Detail = string.Join('-', errors.Select(e => e.Message)),
                Title = "Bad Request",
                Status = 400,
            },
                    ResultStatusCode.BadRequest, errors)
        {
        }

        public BadRequestProblemResult(IEnumerable<ErrorValidation> errors, ProblemDetails content)
            : base(content, ResultStatusCode.BadRequest, errors.ToList())
        {
        }
    }
}
