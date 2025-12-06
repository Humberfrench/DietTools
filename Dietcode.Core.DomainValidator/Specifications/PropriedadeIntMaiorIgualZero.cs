using Dietcode.Core.DomainValidator.Interfaces;
using System.Linq.Expressions;

namespace Dietcode.Core.DomainValidator.Specifications
{
    /// <summary>
    /// Especificação que valida se uma propriedade inteira da entidade é maior que zero.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeIntMaiorQueZero<T> : ISpecification<T>
    {
        private readonly Func<T, int> _propAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeIntMaiorQueZero{T}"/>.
        /// </summary>
        /// <param name="propriedade">
        /// Expressão lambda que representa a propriedade inteira da entidade.
        /// </param>
        public PropriedadeIntMaiorQueZero(Expression<Func<T, int>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }

        /// <summary>
        /// Verifica se o valor da propriedade é maior que zero.
        /// </summary>
        /// <param name="entidade">Instância da entidade a ser validada.</param>
        /// <returns>
        /// True se o valor for maior que zero; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            var valor = _propAccessor(entidade);
            return valor > 0;
        }
    }
}
