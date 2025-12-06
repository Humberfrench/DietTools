using System.Resources;

namespace Dietcode.Api.Core.Results
{
    public class ErrorBuilder
    {
        public ResourceManager _resourceManager;
        private readonly List<ErrorValidation> _errors;

        public IReadOnlyList<ErrorValidation> Errors { get { return _errors; } }

        public bool HasErrors { get { return _errors.Count != 0; } }

        public ErrorBuilder()
        {
            _errors = [];
            _resourceManager = new ResourceManager("ErrorMessages", typeof(ErrorBuilder).Assembly);
        }

        public ErrorBuilder AddError(ErrorValidation error)
        {
            _errors.Add(error);
            return this;
        }

        public ErrorBuilder AddError(Enum code)
        {
            var error = GetError(code);

            _errors.Add(error);

            return this;
        }

        public ErrorValidation GetError(Enum @enum)
        {
            var errorMessage = _resourceManager.GetString(@enum.ToString()) ?? "Unknown Error";

            var err = new ErrorValidation((Convert.ToInt32(@enum)).ToString("000"), errorMessage);

            return err;
        }

        public ErrorValidation GetError(Enum @enum, params object[] args)
        {
            var error = GetError(@enum);

            error.Message = string.Format(error.Message, args);

            return error;
        }

        public List<ErrorValidation> ToList()
        {
            return _errors;
        }
    }
}
