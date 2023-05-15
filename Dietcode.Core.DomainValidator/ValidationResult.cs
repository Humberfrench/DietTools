using System.Net;
using Dietcode.Core.DomainValidator.Interfaces;
using Dietcode.Core.DomainValidator.ObjectValue;

namespace Dietcode.Core.DomainValidator
{
    [Serializable]
    public class ValidationResult : ValidationResultBase, IValidationResult
    {

        public ValidationResult()
        {
            Entries = new List<Entries>();
            Mensagem = string.Empty;
            Retorno = new object();
        }

        public void AddEntries(Entries entries)
        {
            Entries.Add(entries);
        }

        public List<Entries> Entries { get; set; }


        public object Retorno { get; set; }


        //conversão
        public ValidationResult<T> Converter<T>() where T : new()
        {

            var newValidationResult = new ValidationResult<T>
            {
                CodigoMessagem = CodigoMessagem,
                Mensagem = Mensagem,
                StatusCode = StatusCode,
            };

            newValidationResult.GetErros(errors);

            try
            {
                newValidationResult.Retorno = (T)Retorno;
            }
            catch
            {
                newValidationResult.Add("Erro ao efetuar a conversão do retorno. É o mesmo tipo que deseja converter?");
            }
            return newValidationResult;
        }


    }
}

