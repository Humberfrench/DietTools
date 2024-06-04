using System.Net;

namespace Dietcode.Core.DomainValidator.Interfaces
{
    public interface IValidationResult
    {
        HttpStatusCode StatusCode { get; set; }
        IList<ValidationError> Erros { get; }
        void Add(ValidationError error);
        void Add(string error, bool erro = true);
        void Add(int codigo, string error, bool erro = true);
        void AddError(string error);
        void AddWarning(string error);
        void Remove(ValidationError error);
        public string Mensagem { get; set; }
        public int CodigoMessagem { get; set; }
        public bool Warning { get; }

    }
}
