using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dietcode.Core.DomainValidator
{
    [Serializable]
    public class ValidationResult<T> : ValidationResultBase, IValidationResult where T : new()
    {
        private JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 4,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public ValidationResult()
        {
            Retorno = new T();
            Mensagem = string.Empty;
        }

        public ValidationResult(T content)
        {
            Retorno = content;
            Mensagem = string.Empty;
        }

        public void Add(params ValidationResult<T>[] validationResults)
        {
            if (validationResults != null)
            {
                foreach (var result in validationResults)
                {
                    if (result != null)
                    {
                        errors.AddRange(result.errors);
                    }
                }
            }
        }

        public T Retorno { get; set; }

        public ValidationResult Converter()
        {
            var newValidationResult = new ValidationResult
            {
                CodigoMessagem = CodigoMessagem,
                Mensagem = Mensagem,
                StatusCode = StatusCode,
            };

            newValidationResult.GetErros(errors);

            try
            {
                newValidationResult.Retorno = Retorno ?? new object();
            }
            catch
            {
                newValidationResult.Add("Erro ao efetuar a conversão do retorno. É o mesmo tipo que deseja converter?");
            }
            return newValidationResult;
        }

        // Usar de Model para ViewModel e Vice Versa.
        public ValidationResult<T1> Converter<T1>() where T1 : new()
        {
            var json = JsonSerializer.Serialize(Retorno, jsonOptions);
            var retorno = new T1();

            var newValidationResult = new ValidationResult<T1>
            {
                CodigoMessagem = CodigoMessagem,
                Mensagem = Mensagem,
                StatusCode = StatusCode,
            };

            try
            {
                newValidationResult.GetErros(errors);
                retorno = JsonSerializer.Deserialize<T1>(json, jsonOptions);
            }
            catch
            {
                newValidationResult.Add("Erro ao efetuar a conversão do retorno. É o mesmo tipo que deseja converter?");
            }

            return newValidationResult;
        }
    }
}
