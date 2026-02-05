namespace Dietcode.Api.Core.Results.Interfaces
{
    public interface IErrorResult
    {
        IEnumerable<ErrorValidation> Errors { get; }
    }
}
