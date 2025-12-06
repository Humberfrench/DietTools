using Dietcode.Core.DomainValidator.Interfaces;
using System.Linq.Expressions;

namespace Dietcode.Core.DomainValidator.Specifications
{
    /// <summary>
    /// Especificação que valida se uma propriedade inteira da entidade é maior ou igual a zero.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeIntMaiorIgualZero<T> : ISpecification<T>
    {
        private readonly Func<T, int> _propAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeIntMaiorIgualZero{T}"/>.
        /// </summary>
        /// <param name="propriedade">
        /// Expressão lambda que representa a propriedade inteira da entidade.
        /// </param>
        public PropriedadeIntMaiorIgualZero(Expression<Func<T, int>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }

        /// <summary>
        /// Verifica se o valor da propriedade é maior ou igual a zero.
        /// </summary>
        /// <param name="entidade">Instância da entidade a ser validada.</param>
        /// <returns>
        /// True se o valor for maior ou igual a zero; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            var valor = _propAccessor(entidade);
            return valor >= 0;
        }
    }
}
