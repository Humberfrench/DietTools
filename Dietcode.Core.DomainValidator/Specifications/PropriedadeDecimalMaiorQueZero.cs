using Dietcode.Core.DomainValidator.Interfaces;
using System.Linq.Expressions;

namespace Dietcode.Core.DomainValidator.Specifications
{
    /// <summary>
    /// Especificação que valida se uma propriedade decimal da entidade é maior ou igual a zero.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeDecimalMaiorIgualZero<T> : ISpecification<T>
    {
        private readonly Func<T, decimal> _propAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeDecimalMaiorIgualZero{T}"/>.
        /// </summary>
        /// <param name="propriedade">
        /// Expressão lambda que representa a propriedade decimal da entidade.
        /// </param>
        public PropriedadeDecimalMaiorIgualZero(Expression<Func<T, decimal>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }

        /// <summary>
        /// Verifica se o valor da propriedade decimal é maior ou igual a zero.
        /// </summary>
        /// <param name="entidade">Instância da entidade a ser validada.</param>
        /// <returns>
        /// True se o valor for maior ou igual a zero; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            var valor = _propAccessor(entidade);
            return valor >= 0m;
        }
    }
}
