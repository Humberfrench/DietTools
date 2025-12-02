using Dietcode.Core.DomainValidator.Interfaces;

namespace Dietcode.Core.DomainValidator
{
    public class Rule<TEntity> : IRule<TEntity> where TEntity : class, new()
    {
        private readonly ISpecification<TEntity> _specificationRule;

        public string MensagemErro { get; private set; }

        public Rule(ISpecification<TEntity> rule, string mensagemErro)
        {
            _specificationRule = rule;
            MensagemErro = mensagemErro;
        }

        public bool Validar(TEntity entity)
        {
            return _specificationRule.IsSatisfiedBy(entity);
        }
    }
}
