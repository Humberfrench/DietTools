using Dietcode.Core.Domain.Rules.Interfaces;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Dietcode.Core.Domain.Rules.Specifications
{
    /// <summary>
    /// Especificação que valida se uma propriedade string da entidade representa um número válido (apenas dígitos).
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeNumeroStringValido<T> : ISpecification<T>
    {
        private readonly Func<T, string> _stringAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeNumeroStringValido{T}"/>.
        /// </summary>
        /// <param name="propriedade">
        /// Expressão lambda que representa a propriedade string da entidade a ser verificada.
        /// </param>
        public PropriedadeNumeroStringValido(Expression<Func<T, string>> propriedade)
        {
            _stringAccessor = propriedade.Compile();
        }

        /// <summary>
        /// Verifica se o valor da propriedade string não é nulo, vazio ou apenas espaços,
        /// e contém apenas caracteres numéricos (0-9).
        /// </summary>
        /// <param name="entidade">Entidade que será validada.</param>
        /// <returns>
        /// True se a string for um número válido (apenas dígitos); caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            string valor = _stringAccessor(entidade);

            // Verifica se a string não é nula ou vazia e se contém apenas dígitos
            return !string.IsNullOrWhiteSpace(valor) && Regex.IsMatch(valor, @"^\d+$");
        }
    }
}
