using System.Text.Json;

namespace Dietcode.Core.DomainValidator
{
    /// <summary>
    /// Representa o resultado de uma validação sem retorno fortemente tipado.
    /// Herda de <see cref="ValidationResult{object}"/>.
    /// </summary>
    [Serializable]
    public class ValidationResult : ValidationResult<object>
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ValidationResult"/>.
        /// </summary>
        public ValidationResult() : base()
        {
        }
        public T? TryReturnAs<T>()
        {
            if (Retorno is T value)
                return value;

            if (!errors.Any(e => e.Message.Contains(typeof(T).Name)))
                AddError($"Conversão inválida: o retorno não é do tipo {typeof(T).Name}.");

            return default;
        }

    }

    /// <summary>
    /// Representa o resultado de uma validação com um objeto de retorno fortemente tipado.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto de retorno.</typeparam>
    [Serializable]
    public class ValidationResult<T> : ValidationResultBase where T : new()
    {
        /// <summary>
        /// Objeto de retorno da validação.
        /// </summary>
        public T Retorno { get; set; } = new();

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ValidationResult{T}"/> com valor padrão.
        /// </summary>
        public ValidationResult() : base()
        {
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ValidationResult{T}"/> com um valor de retorno fornecido.
        /// </summary>
        /// <param name="retorno">Valor a ser atribuído ao retorno.</param>
        public ValidationResult(T retorno) : base()
        {
            Retorno = retorno;
        }

        /// <summary>
        /// Converte esta instância para <see cref="ValidationResult{U}"/>, copiando erros e informações de status.
        /// A conversão do objeto de retorno é feita via serialização JSON.
        /// </summary>
        /// <typeparam name="U">Tipo de destino para o novo objeto de retorno.</typeparam>
        /// <returns>Instância de <see cref="ValidationResult{U}"/> com os dados convertidos.</returns>
        public ValidationResult<U> Converter<U>() where U : new()
        {
            var novo = new ValidationResult<U>
            {
                StatusCode = StatusCode
            };

            // Copiar erros do resultado atual
            novo.Add(this);

            try
            {
                // Conversão do retorno via JSON
                string json = JsonSerializer.Serialize(Retorno);
                novo.Retorno = JsonSerializer.Deserialize<U>(json!)!;
            }
            catch
            {
                novo.AddError("Erro ao efetuar a conversão do retorno. É o mesmo tipo que deseja converter?");
            }

            return novo;
        }
    }
}
