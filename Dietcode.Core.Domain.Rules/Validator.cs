using Dietcode.Core.Domain.Rules.Interfaces;

namespace Dietcode.Core.Domain.Rules
{
    /// <summary>
    /// Classe abstrata base para validação de entidades.
    /// Implementa o padrão de projeto Strategy com regras de validação dinâmicas.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade a ser validada.</typeparam>
    public abstract class Validator<TEntity> : IValidator<TEntity> where TEntity : class
    {
        private readonly Dictionary<string, IRule<TEntity>> validations = new Dictionary<string, IRule<TEntity>>();

        /// <summary>
        /// Adiciona uma regra de validação à lista de regras.
        /// </summary>
        /// <param name="nomeRegra">Nome identificador da regra.</param>
        /// <param name="rule">Instância da regra de validação.</param>
        protected virtual void AdicionarRegra(string nomeRegra, IRule<TEntity> rule)
        {
            validations.Add(nomeRegra, rule);
        }

        /// <summary>
        /// Remove uma regra de validação pelo nome.
        /// </summary>
        /// <param name="nomeRegra">Nome da regra a ser removida.</param>
        protected virtual void RemoverRegra(string nomeRegra)
        {
            validations.Remove(nomeRegra);
        }

        /// <summary>
        /// Valida a entidade com base nas regras adicionadas.
        /// </summary>
        /// <param name="entity">Entidade a ser validada.</param>
        /// <returns>Resultado da validação contendo os erros, se houver.</returns>
        public virtual ValidatorRules Validar(TEntity entity)
        {
            ValidatorRules result = new();

            foreach (var x in validations.Keys)
            {
                var rule = validations[x];

                if (!rule.Validar(entity))
                {
                    result.Errors.Add(rule.MensagemErro);
                }
            }

            return result;
        }

        /// <summary>
        /// Recupera uma regra de validação pelo nome.
        /// </summary>
        /// <param name="nomeRegra">Nome da regra desejada.</param>
        /// <returns>Instância da regra, se encontrada; caso contrário, null.</returns>
        protected IRule<TEntity> ObterRegra(string nomeRegra)
        {
            validations.TryGetValue(nomeRegra, out var rule);
            return rule!;
        }
    }
}
