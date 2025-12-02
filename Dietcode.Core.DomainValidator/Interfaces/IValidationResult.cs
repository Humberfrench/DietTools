using System.Net;

namespace Dietcode.Core.DomainValidator.Interfaces
{
    public interface IValidationResult
    {
        HttpStatusCode StatusCode { get; set; }

        IReadOnlyList<ValidationError> Erros { get; }

        void AddError(string message);
        void AddError(string message, int codigo);
        void AddError(string message, HttpStatusCode statusCode);

        void Add(IValidationResult other);

        string Mensagem { get; set; }
        int CodigoMensagem { get; set; }

        bool Valid { get; }
        bool Invalid { get; }
    }

}
