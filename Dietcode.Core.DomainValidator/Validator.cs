using Dietcode.Core.DomainValidator.Interfaces;

namespace Dietcode.Core.DomainValidator
{
    public abstract class Validator<TEntity> : IValidator<TEntity> where TEntity : class
    {
        private readonly Dictionary<string, IRule<TEntity>> validations = new Dictionary<string, IRule<TEntity>>();

        protected virtual void AdicionarRegra(string nomeRegra, IRule<TEntity> rule)
        {
            validations.Add(nomeRegra, rule);
        }

        protected virtual void RemoverRegra(string nomeRegra)
        {
            validations.Remove(nomeRegra);
        }

        public virtual ValidationResult Validar(TEntity entity)
        {
            var result = new ValidationResult();

            foreach (var x in validations.Keys)
            {
                var rule = validations[x];
                if (!rule.Validar(entity))
                    result.Add(new ValidationError(rule.MensagemErro));
            }

            return result;
        }

        protected IRule<TEntity> ObterRegra(string nomeRegra)
        {
            IRule<TEntity> rule;
            validations.TryGetValue(nomeRegra, out rule);
            return rule;
        }



    }
}
