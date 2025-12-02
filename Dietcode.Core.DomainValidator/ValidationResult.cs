using Dietcode.Core.DomainValidator.Interfaces;
using Dietcode.Core.DomainValidator.ObjectValue;
using System.Text.Json;

namespace Dietcode.Core.DomainValidator
{
    [Serializable]
    public class ValidationResult : ValidationResult<object>
    {
        public ValidationResult() : base()
        {
        }
    }

    [Serializable]
    public class ValidationResult<T> : ValidationResultBase where T : new()
    {
        public T Retorno { get; set; } = new();

        public ValidationResult() : base()
        {
        }

        public ValidationResult(T retorno) : base()
        {
            Retorno = retorno;
        }

        /// <summary>
        /// Converte este ValidationResult<T> para ValidationResult<U>,
        /// mantendo erros, status e mensagens.
        /// </summary>
        public ValidationResult<U> Converter<U>() where U : new()
        {
            var novo = new ValidationResult<U>
            {
                CodigoMensagem = CodigoMensagem,
                Mensagem = Mensagem,
                StatusCode = StatusCode
            };

            // Copiar erros
            novo.Add(this);

            try
            {
                // Conversão via JSON para manter compatibilidade
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

