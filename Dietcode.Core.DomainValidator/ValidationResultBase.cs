using Dietcode.Core.DomainValidator.Interfaces;
using System.Net;

namespace Dietcode.Core.DomainValidator
{
    [Serializable]
    public abstract class ValidationResultBase
    {
        protected readonly List<ValidationError> errors = new();

        public HttpStatusCode StatusCode { get; set; }
        public int CodigoMensagem { get; set; }
        public string Mensagem { get; set; } = string.Empty;

        public IReadOnlyList<ValidationError> Erros => errors;

        // Agora não existe mais warning. Erro = erro.
        public bool Valid => !errors.Any();
        public bool Invalid => errors.Any();

        // -------------------------------------------------------
        // Métodos de adição de erros
        // -------------------------------------------------------

        public void AddError(string message)
        {
            errors.Add(new ValidationError(message));
        }

        public void AddError(string message, int codigo)
        {
            errors.Add(new ValidationError(codigo, message));
        }

        public void AddError(string message, HttpStatusCode statusCode)
        {
            errors.Add(new ValidationError(message));
            StatusCode = statusCode;
        }

        // Recebe erros de outro ValidationResultBase
        public void Add(string message)
        {
            errors.Add(new ValidationError(message));
        }

        public void Add(ValidationResultBase other)
        {
            if (other != null)
                errors.AddRange(other.errors);
        }
    }
}
