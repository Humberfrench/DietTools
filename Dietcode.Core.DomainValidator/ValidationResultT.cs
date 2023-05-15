using System.Net;
using Dietcode.Core.DomainValidator.Interfaces;
using Dietcode.Core.DomainValidator.ObjectValue;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Dietcode.Core.DomainValidator
{
    [Serializable]
    public class ValidationResult<T> : ValidationResultBase, IValidationResult where T : new()
    {
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = 4,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };

        public ValidationResult() 
        {
            Retorno = new T();
            Mensagem = string.Empty;
        }

        public void Add(params ValidationResult<T>[] validationResults)
        {
            if (validationResults != null)
            {
                foreach (var result in from result in validationResults
                                       where result != null
                                       select result)
                {
                    errors.AddRange(result.errors);
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


        //Usar de Model para ViewModel e Vice Versa.
        public ValidationResult<T1> Converter<T1>() where T1 : new()

        {
            var json = JsonConvert.SerializeObject(Retorno, jsonSettings);
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
                retorno = JsonConvert.DeserializeObject<T1>(json,jsonSettings);
            }
            catch
            {
                newValidationResult.Add("Erro ao efetuar a conversão do retorno. É o mesmo tipo que deseja converter?");
            }

            return newValidationResult;

        }


    }
}

