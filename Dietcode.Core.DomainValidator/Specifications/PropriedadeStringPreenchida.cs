using Dietcode.Core.DomainValidator.Interfaces;
using System.Linq.Expressions;

namespace Dietcode.Core.DomainValidator.Specifications
{
    /// <summary>
    /// Especificação que valida se uma propriedade string da entidade está preenchida.
    /// Considera nulo, vazio ou apenas espaços como não preenchido.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeStringPreenchida<T> : ISpecification<T>
    {
        private readonly Func<T, string> _propAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeStringPreenchida{T}"/>.
        /// </summary>
        /// <param name="propriedade">
        /// Expressão lambda que representa a propriedade string da entidade a ser verificada.
        /// </param>
        public PropriedadeStringPreenchida(Expression<Func<T, string>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }

        /// <summary>
        /// Verifica se a propriedade string está preenchida (não nula, não vazia e não composta apenas por espaços).
        /// </summary>
        /// <param name="entidade">Entidade que será validada.</param>
        /// <returns>
        /// True se a propriedade estiver preenchida; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            var valor = _propAccessor(entidade);
            return !string.IsNullOrWhiteSpace(valor);
        }
    }
}
