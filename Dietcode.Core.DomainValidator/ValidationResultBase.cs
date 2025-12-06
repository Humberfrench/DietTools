using Dietcode.Core.DomainValidator.ObjectValue;
using System.Net;

namespace Dietcode.Core.DomainValidator
{
    /// <summary>
    /// Classe base para resultados de validação, agregando erros,
    /// código de status HTTP, mensagens e metadados auxiliares.
    /// </summary>
    [Serializable]
    public abstract class ValidationResultBase
    {
        /// <summary>
        /// Lista interna que armazena todos os erros de validação.
        /// </summary>
        protected readonly List<ValidationError> errors = new();

        /// <summary>
        /// Código HTTP associado ao resultado da operação.
        /// Pode ser útil em validações em APIs.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Informações referentes a entradas modificadas,
        /// como IDs criados ou atualizados em operações de persistência (EF, Dapper etc.).
        /// </summary>
        public Entries Entries { get; set; } = new();

        /// <summary>
        /// Código numérico opcional associado à mensagem do resultado.
        /// Pode representar códigos de erro internos.
        /// </summary>
        public int CodigoMensagem { get; set; }

        /// <summary>
        /// Mensagem principal associada ao resultado.
        /// </summary>
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Lista somente leitura contendo todos os erros coletados.
        /// </summary>
        public IReadOnlyList<ValidationError> Erros => errors;

        /// <summary>
        /// Indica se o resultado é válido (sem erros).
        /// </summary>
        public bool Valid => errors.Count == 0;

        /// <summary>
        /// Indica se o resultado é inválido (possui erros).
        /// </summary>
        public bool Invalid => errors.Count != 0;

        // -------------------------------------------------------
        // Métodos de adição de erros
        // -------------------------------------------------------

        /// <summary>
        /// Adiciona um erro simples à coleção de erros.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        public void AddError(string message)
        {
            errors.Add(new ValidationError(message));
        }

        /// <summary>
        /// Adiciona um erro com código numérico associado.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        /// <param name="codigo">Código do erro.</param>
        public void AddError(string message, int codigo)
        {
            errors.Add(new ValidationError(codigo, message));
        }

        /// <summary>
        /// Adiciona um erro e define um código HTTP para o resultado.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        /// <param name="statusCode">Código HTTP a ser associado ao erro.</param>
        public void AddError(string message, HttpStatusCode statusCode)
        {
            errors.Add(new ValidationError(message));
            StatusCode = statusCode;
        }

        /// <summary>
        /// Adiciona um erro simples (alias de AddError).
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        public void Add(string message)
        {
            errors.Add(new ValidationError(message));
        }

        /// <summary>
        /// Adiciona todos os erros presentes em outro <see cref="ValidationResultBase"/>.
        /// </summary>
        /// <param name="other">Objeto contendo erros que devem ser agregados.</param>
        public void Add(ValidationResultBase other)
        {
            if (other != null)
                errors.AddRange(other.errors);
        }
    }
}
