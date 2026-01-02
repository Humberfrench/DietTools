using Dietcode.Core.Domain.Rules.Interfaces;
using System.Linq.Expressions;

namespace Dietcode.Core.Domain.Rules.Specifications
{
    /// <summary>
    /// Especificação que verifica se uma propriedade decimal de uma entidade é maior que zero.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeDecimalMaiorQueZero<T> : ISpecification<T>
    {
        private readonly Func<T, decimal> _propAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeDecimalMaiorQueZero{T}"/>.
        /// </summary>
        /// <param name="propriedade">
        /// Expressão lambda que aponta para a propriedade decimal da entidade que será validada.
        /// </param>
        public PropriedadeDecimalMaiorQueZero(Expression<Func<T, decimal>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }

        /// <summary>
        /// Verifica se a propriedade decimal da entidade é maior que zero.
        /// </summary>
        /// <param name="entidade">Instância da entidade a ser validada.</param>
        /// <returns>
        /// True se o valor da propriedade for maior que zero; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            var valor = _propAccessor(entidade);
            return valor > 0m;
        }
    }
}
