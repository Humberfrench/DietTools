namespace Dietcode.Core.DomainValidator
{
    /// <summary>
    /// Representa um erro de validação ocorrido durante o processo de validação de uma entidade.
    /// </summary>
    [Serializable]
    public class ValidationError
    {
        /// <summary>
        /// Código identificador do erro.
        /// </summary>
        public int Codigo { get; set; }

        /// <summary>
        /// Mensagem descritiva do erro.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ValidationError"/> com valores padrão.
        /// </summary>
        public ValidationError()
        {
            Codigo = 0;
            Message = string.Empty;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ValidationError"/> com uma mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem descritiva do erro.</param>
        public ValidationError(string message)
        {
            Codigo = 0;
            Message = message;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ValidationError"/> com código e mensagem.
        /// </summary>
        /// <param name="codigo">Código do erro.</param>
        /// <param name="message">Mensagem descritiva do erro.</param>
        public ValidationError(int codigo, string message)
        {
            Codigo = codigo;
            Message = message;
        }
    }
}
