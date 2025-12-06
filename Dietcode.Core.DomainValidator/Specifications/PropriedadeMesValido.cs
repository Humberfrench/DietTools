using Dietcode.Core.DomainValidator.Interfaces;
using System.Linq.Expressions;

namespace Dietcode.Core.DomainValidator.Specifications
{
    /// <summary>
    /// Especificação que valida se uma propriedade inteira da entidade representa um mês válido (1 a 12).
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeMesValido<T> : ISpecification<T>
    {
        private readonly Func<T, int> _mesAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeMesValido{T}"/>.
        /// </summary>
        /// <param name="mes">
        /// Expressão lambda que representa a propriedade da entidade que contém o número do mês.
        /// </param>
        public PropriedadeMesValido(Expression<Func<T, int>> mes)
        {
            _mesAccessor = mes.Compile();
        }

        /// <summary>
        /// Verifica se o valor da propriedade representa um mês válido (entre 1 e 12).
        /// </summary>
        /// <param name="entidade">Entidade a ser validada.</param>
        /// <returns>
        /// True se o mês estiver no intervalo de 1 a 12; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            int mes = _mesAccessor(entidade);
            return mes >= 1 && mes <= 12;
        }
    }
}
