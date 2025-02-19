using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dietcode.Api.Core.Results
{
    public abstract partial class AppServiceBase
    {
        protected ErrorBuilder ErrorBuilder;

        protected AppServiceBase(ErrorBuilder builder)
        {
            ErrorBuilder = builder;
        }

        protected AppServiceBase()
        {
            ErrorBuilder = new ErrorBuilder();
        }

        public OkResult Ok()
        {
            return new OkResult();
        }

        public OkResult<TContent> Ok<TContent>(TContent content)
        {
            return new OkResult<TContent>(content);
        }

        public CreatedResult Created()
        {
            return new CreatedResult();
        }

        public CreatedResult<TContent> Created<TContent>(TContent content, object identifier)
        {
            return new CreatedResult<TContent>(content, identifier);
        }

        public AcceptedResult<TContent> Accepted<TContent>(TContent content, object identifier)
        {
            return new AcceptedResult<TContent>(content, identifier);
        }

        public AcceptedResult Accepted()
        {
            return new AcceptedResult();
        }

        public BadRequestResult BadRequest(string error)
        {
            return new BadRequestResult(new ErrorValidation(null!, error));
        }

        public BadRequestResult<T> BadRequest<T>(string error, T content) where T : class, new()
        {
            return new BadRequestResult<T>(new ErrorValidation(null!, error), content);
        }

        public BadRequestProblemResult BadRequestProblem(Enum error)
        {
            return new BadRequestProblemResult(ErrorBuilder.GetError(error));
        }

        public BadRequestProblemResult BadRequestProblem(string error)
        {
            var problem = new ProblemDetails
            {
                Detail = error,
                Status = 400,
                Title = "Bad Request"
            };
            return new BadRequestProblemResult(new ErrorValidation(null!, error), problem);
        }

        public BadRequestResult BadRequest(Enum error)
        {
            return new BadRequestResult(ErrorBuilder.GetError(error));
        }

        public BadRequestResult BadRequest(Enum error, params object[] args)
        {
            return new BadRequestResult(ErrorBuilder.GetError(error, args));
        }

        public BadRequestResult BadRequest(ErrorValidation error)
        {
            return new BadRequestResult(error);
        }

        public BadRequestResult BadRequest(IEnumerable<ErrorValidation> errorList)
        {
            return new BadRequestResult(errorList);
        }

        public UnprocessableEntityResult UnprocessableEntity(ErrorValidation error)
        {
            return new UnprocessableEntityResult(error);
        }

        public UnprocessableEntityResult UnprocessableEntity(IEnumerable<ErrorValidation> errorList)
        {
            return new UnprocessableEntityResult(errorList);
        }

        public NotFoundResult NotFound(string error)
        {
            return new NotFoundResult(new ErrorValidation(null!, error));
        }

        public NotFoundResult NotFound(Enum error)
        {
            return new NotFoundResult(ErrorBuilder.GetError(error));
        }

        public NotFoundResult NotFound(Enum error, params object[] args)
        {
            return new NotFoundResult(ErrorBuilder.GetError(error, args));
        }

        public NotFoundResult NotFound(ErrorValidation error)
        {
            return new NotFoundResult(error);
        }

        public NotFoundResult NotFound(IEnumerable<ErrorValidation> errorList)
        {
            return new NotFoundResult(errorList);
        }

        public TimeOutResult TimeOut(string error)
        {
            return new TimeOutResult(new ErrorValidation(null!, error));
        }

        public TimeOutResult TimeOut(Enum error)
        {
            return new TimeOutResult(ErrorBuilder.GetError(error));
        }

        public TimeOutResult TimeOut(Enum error, params object[] args)
        {
            return new TimeOutResult(ErrorBuilder.GetError(error, args));
        }

        public TimeOutResult TimeOut(ErrorValidation error)
        {
            return new TimeOutResult(error);
        }

        public TimeOutResult TimeOut(IEnumerable<ErrorValidation> errorList)
        {
            return new TimeOutResult(errorList);
        }

        public ConflictResult Conflict(string error)
        {
            return new ConflictResult(new ErrorValidation(null!, error));
        }

        public ConflictResult Conflict(Enum error)
        {
            return new ConflictResult(ErrorBuilder.GetError(error));
        }

        public ConflictResult Conflict(Enum error, params object[] args)
        {
            return new ConflictResult(ErrorBuilder.GetError(error, args));
        }

        public ConflictResult Conflict(ErrorValidation error)
        {
            return new ConflictResult(error);
        }

        public ConflictResult Conflict(IEnumerable<ErrorValidation> errorList)
        {
            return new ConflictResult(errorList);
        }

        public NotAcceptableResult NotAcceptable(IEnumerable<ErrorValidation> errorList)
        {
            return new NotAcceptableResult(errorList);
        }

        public NotAcceptableResult NotAcceptable(ErrorValidation error)
        {
            return new NotAcceptableResult(error);
        }

        public NotAcceptableResult<TContent> NotAcceptable<TContent>(TContent content, ErrorValidation error)
        {
            return new NotAcceptableResult<TContent>(content, error);
        }
    
        public NotAcceptableResult<TContent> NotAcceptable<TContent>(TContent content, IEnumerable<ErrorValidation> errorList)
        {
            return new NotAcceptableResult<TContent>(content, errorList);
        }

        public ForbiddenResult Forbidden()
        {
            return new ForbiddenResult();
        }

        public InternalServerErrorResult InternalServerError(Exception ex)
        {
            return new InternalServerErrorResult(ex);
        }
    }
}
