using Dietcode.Core.Domain.Rules.Interfaces;
using System.Linq.Expressions;

namespace Dietcode.Core.Domain.Rules.Specifications
{
    /// <summary>
    /// Especificação que valida se pelo menos uma das propriedades string especificadas está preenchida.
    /// Útil para validar campos opcionais onde ao menos um deve ser informado.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class RequisitoMinimoPreenchido<T> : ISpecification<T>
    {
        private readonly Func<T, string>[] _propriedades;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="RequisitoMinimoPreenchido{T}"/>.
        /// </summary>
        /// <param name="propriedades">
        /// Lista de expressões que apontam para as propriedades string da entidade que devem ser avaliadas.
        /// </param>
        public RequisitoMinimoPreenchido(params Expression<Func<T, string>>[] propriedades)
        {
            _propriedades = propriedades.Select(p => p.Compile()).ToArray();
        }

        /// <summary>
        /// Verifica se pelo menos uma das propriedades está preenchida (não é nula, vazia ou composta apenas por espaços).
        /// </summary>
        /// <param name="entidade">Instância da entidade a ser validada.</param>
        /// <returns>
        /// True se ao menos uma propriedade estiver preenchida; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            int preenchido = 0;
            foreach (var prop in _propriedades)
            {
                if (!string.IsNullOrWhiteSpace(prop(entidade)))
                {
                    preenchido++;
                }
            }
            return preenchido > 0;
        }
    }
}
