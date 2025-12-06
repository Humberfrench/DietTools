using Dietcode.Core.DomainValidator.Interfaces;

namespace Dietcode.Core.DomainValidator
{
    /// <summary>
    /// Implementa uma regra de validação baseada em uma especificação.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade a ser validada.</typeparam>
    public class Rule<TEntity> : IRule<TEntity> where TEntity : class, new()
    {
        private readonly ISpecification<TEntity> _specificationRule;

        /// <summary>
        /// Mensagem de erro a ser exibida caso a validação falhe.
        /// </summary>
        public string MensagemErro { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Rule{TEntity}"/>.
        /// </summary>
        /// <param name="rule">Especificação que define a regra de validação.</param>
        /// <param name="mensagemErro">Mensagem de erro associada à falha na validação.</param>
        public Rule(ISpecification<TEntity> rule, string mensagemErro)
        {
            _specificationRule = rule;
            MensagemErro = mensagemErro;
        }

        /// <summary>
        /// Valida a entidade de acordo com a especificação fornecida.
        /// </summary>
        /// <param name="entity">Entidade a ser validada.</param>
        /// <returns>Retorna true se a entidade satisfaz a especificação; caso contrário, false.</returns>
        public bool Validar(TEntity entity)
        {
            return _specificationRule.IsSatisfiedBy(entity);
        }
    }
}
