using Dietcode.Core.DomainValidator.Interfaces;
using System.Net;

namespace Dietcode.Core.DomainValidator
{
    [Serializable]
    public abstract class ValidationResultBase : IValidationResult
    {
        protected readonly List<ValidationError> errors = new List<ValidationError>();

        public ValidationResultBase()
        {
            errors = new List<ValidationError>();
            Mensagem = "";
        }

        public HttpStatusCode StatusCode { get; set; }

        public IList<ValidationError> Erros => errors;

        public int CodigoMessagem { get; set; }

        public string Mensagem { get; set; }

        public bool Valid => !errors.Any(vr => vr.Erro);

        public bool Invalid => errors.Any(vr => vr.Erro);

        public bool Warning
        {
            get
            {
                return errors.Count(vr => vr.Erro) != Erros.Count;
            }
        }

        public void Add(ValidationError error)
        {
            errors.Add(error);
        }

        public void Add(params ValidationResult[] validationResults)
        {
            if (validationResults != null)
            {
                foreach (var result in from result in validationResults
                                       where result != null
                                       select result)
                {
                    errors.AddRange(result.Erros);
                }
            }
        }

        public void Add(string error, bool erro = true)
        {
            var validationErro = new ValidationError(error, erro);
            errors.Add(validationErro);

        }
        public void Add(string error, HttpStatusCode statusCode)
        {
            var validationErro = new ValidationError(error, true);
            errors.Add(validationErro);
            StatusCode = statusCode;

        }

        public void Add(int codigo, string error, bool erro = true)
        {
            var validationErro = new ValidationError(codigo, error, erro);
            errors.Add(validationErro);
        }

        public void AddError(string error)
        {
            var validationErro = new ValidationError(error, true);
            errors.Add(validationErro);
        }

        public void AddError(string error, HttpStatusCode statusCode)
        {
            var validationErro = new ValidationError(error, true);
            errors.Add(validationErro);
            StatusCode = statusCode;
        }

        public void AddWarning(string error)
        {
            var validationErro = new ValidationError(error, false);
            errors.Add(validationErro);
        }

        public void Remove(ValidationError error)
        {
            if (errors.Contains(error))
            {
                errors.Remove(error);
            }
        }

        public void GetErros(IList<ValidationError> erros)
        {
            GetErros(erros.ToList());
        }

        public void GetErros(List<ValidationError> erros)
        {
            erros.ForEach(e => Add(e));
        }

    }
}
