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
        public List<Entries> Entries { get; set; } = [];

        /// <summary>
        /// Código numérico opcional associado à mensagem do resultado.
        /// Pode representar códigos de erro internos.
        /// </summary>
        [Obsolete("Use a propriedade Mensagens em vez de CodigoMensagem.", true)]
        public int CodigoMensagem { get; set; }

        /// <summary>
        /// Mensagem principal associada ao resultado.
        /// </summary>
        [Obsolete("Use a propriedade Mensagens em vez de Mensagem.", true)]
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem principal associada ao resultado.
        /// </summary>
        public List<MensagemData> Mensagens { get; } = [];

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

        /// <summary>
        /// Indica se o resultado Tem Mensagens. Mensagens não são ERROS.
        /// </summary>
        public bool TemMensagens => Mensagens.Count != 0;

        // =================================================
        //   NOVOS MÉTODOS PADRONIZADOS (RECOMENDADOS)
        // =================================================

        public void AddMensagem(string mensagem)
            => Mensagens.Add(new MensagemData(mensagem));

        public void AddMensagem(int codigo, string mensagem)
            => Mensagens.Add(new MensagemData(codigo, mensagem));

        public void AddMensagem(List<string> mensagemLista)
            => mensagemLista.ForEach(m => Mensagens.Add(new MensagemData(m)));



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
        /// Adiciona um erro simples (alias de AddError).
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        public void Add(ValidationError validationError)
        {
            errors.Add(validationError);
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

        // ------------------------
        // Métodos Retornos de Erros e ou Mensagens Aglutinadas
        // ------------------------
        public string RenderizeErrosAsHtml()
            => RenderListAsHtml(Erros.Select(e => e.Message));

        public string RenderizeErrosAsText()
            => RenderListAsText(Erros.Select(e => e.Message));

        public string RenderizeMensagensAsHtml()
            => RenderListAsHtml(Mensagens.Select(e => e.Mensagem));

        public string RenderizeMensagensAsText()
            => RenderListAsText(Mensagens.Select(e => e.Mensagem));


        // ------------------------
        // Métodos privados auxiliares
        // ------------------------

        private static string RenderListAsHtml(IEnumerable<string> mensagens)
        {
            return string.Concat(mensagens.Select(msg => $"- {msg}<br/>"));
        }

        private static string RenderListAsText(IEnumerable<string> mensagens)
        {
            return string.Concat(mensagens.Select(msg => $"- {msg}{Environment.NewLine}"));
        }
    }
}
