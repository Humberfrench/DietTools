using Microsoft.AspNetCore.Mvc;

namespace Dietcode.Api.Core.Results
{
    public abstract partial class AppServiceBase
    {
        protected ErrorBuilder ErrorBuilder { get; }

        protected AppServiceBase(ErrorBuilder builder)
            => ErrorBuilder = builder;

        protected AppServiceBase()
            => ErrorBuilder = new ErrorBuilder();

        // ---------------------------
        // Central creators
        // ---------------------------

        protected TSuccess Success<TSuccess>(TSuccess result)
            where TSuccess : MethodResult
            => result;

        protected TError Failure<TError>(TError result)
            where TError : MethodResult
            => result;

        // ---------------------------
        // Success results
        // ---------------------------

        public OkResult Ok()
            => Success(new OkResult());

        public OkResult<TContent> Ok<TContent>(TContent content)
            => Success(new OkResult<TContent>(content));

        public CreatedResult Created()
            => Success(new CreatedResult());

        public CreatedResult<TContent> Created<TContent>(TContent content, object id)
            => Success(new CreatedResult<TContent>(content, id));

        public AcceptedResult Accepted()
            => Success(new AcceptedResult());

        public AcceptedResult<TContent> Accepted<TContent>(TContent content, object id)
            => Success(new AcceptedResult<TContent>(content, id));

        // ---------------------------
        // BadRequest
        // ---------------------------

        public BadRequestResult BadRequest(string msg)
            => Failure(new BadRequestResult(new ErrorValidation(null!, msg)));

        public BadRequestResult<T> BadRequest<T>(string msg, T content)
            where T : class, new()
            => Failure(new BadRequestResult<T>(content, new ErrorValidation(null!, msg)));

        public BadRequestResult BadRequest(Enum error)
            => Failure(new BadRequestResult(ErrorBuilder.GetError(error)));

        public BadRequestResult BadRequest(Enum error, params object[] args)
            => Failure(new BadRequestResult(ErrorBuilder.GetError(error, args)));

        public BadRequestResult BadRequest(ErrorValidation error)
            => Failure(new BadRequestResult(error));

        public BadRequestResult BadRequest(IEnumerable<ErrorValidation> errors)
            => Failure(new BadRequestResult(errors));

        // ---------------------------
        // NotFound
        // ---------------------------

        public NotFoundResult NotFound(string msg)
            => Failure(new NotFoundResult(new ErrorValidation(null!, msg)));

        public NotFoundResult NotFound(Enum error)
            => Failure(new NotFoundResult(ErrorBuilder.GetError(error)));

        public NotFoundResult NotFound(Enum error, params object[] args)
            => Failure(new NotFoundResult(ErrorBuilder.GetError(error, args)));

        public NotFoundResult NotFound(ErrorValidation error)
            => Failure(new NotFoundResult(error));

        public NotFoundResult NotFound(IEnumerable<ErrorValidation> errors)
            => Failure(new NotFoundResult(errors));

        // ---------------------------
        // TimeOut
        // ---------------------------

        public TimeOutResult TimeOut(string msg)
            => Failure(new TimeOutResult(new ErrorValidation(null!, msg)));

        public TimeOutResult TimeOut(Enum error)
            => Failure(new TimeOutResult(ErrorBuilder.GetError(error)));

        public TimeOutResult TimeOut(Enum error, params object[] args)
            => Failure(new TimeOutResult(ErrorBuilder.GetError(error, args)));

        public TimeOutResult TimeOut(ErrorValidation error)
            => Failure(new TimeOutResult(error));

        public TimeOutResult TimeOut(IEnumerable<ErrorValidation> errors)
            => Failure(new TimeOutResult(errors));

        // ---------------------------
        // Conflict
        // ---------------------------

        public ConflictResult Conflict(string msg)
            => Failure(new ConflictResult(new ErrorValidation(null!, msg)));

        public ConflictResult Conflict(Enum error)
            => Failure(new ConflictResult(ErrorBuilder.GetError(error)));

        public ConflictResult Conflict(Enum error, params object[] args)
            => Failure(new ConflictResult(ErrorBuilder.GetError(error, args)));

        public ConflictResult Conflict(ErrorValidation error)
            => Failure(new ConflictResult(error));

        public ConflictResult Conflict(IEnumerable<ErrorValidation> errors)
            => Failure(new ConflictResult(errors));

        // ---------------------------
        // NotAcceptable
        // ---------------------------

        public NotAcceptableResult NotAcceptable(ErrorValidation error)
            => Failure(new NotAcceptableResult(error));

        public NotAcceptableResult NotAcceptable(IEnumerable<ErrorValidation> errors)
            => Failure(new NotAcceptableResult(errors));

        public NotAcceptableResult<TContent> NotAcceptable<TContent>(TContent content, ErrorValidation error)
            => Failure(new NotAcceptableResult<TContent>(content, error));

        public NotAcceptableResult<TContent> NotAcceptable<TContent>(TContent content, IEnumerable<ErrorValidation> errors)
            => Failure(new NotAcceptableResult<TContent>(content, errors));

        // ---------------------------
        // Forbidden / InternalServerError
        // ---------------------------

        public ForbiddenResult Forbidden()
            => Failure(new ForbiddenResult());

        public InternalServerErrorResult InternalServerError(Exception ex)
            => Failure(new InternalServerErrorResult(ex));
    }
}
