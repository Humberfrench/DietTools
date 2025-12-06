using Dietcode.Core.DomainValidator.Interfaces;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Dietcode.Core.DomainValidator.Specifications
{
    /// <summary>
    /// Especificação que valida se uma propriedade string da entidade representa um e-mail válido.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeEmailValido<T> : ISpecification<T>
    {
        private readonly Func<T, string> _propAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeEmailValido{T}"/>.
        /// </summary>
        /// <param name="propriedade">
        /// Expressão lambda que representa a propriedade string da entidade que será validada como e-mail.
        /// </param>
        public PropriedadeEmailValido(Expression<Func<T, string>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }

        /// <summary>
        /// Verifica se o valor da propriedade representa um e-mail válido.
        /// </summary>
        /// <param name="entidade">Instância da entidade a ser validada.</param>
        /// <returns>
        /// True se o valor for um e-mail válido; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            var email = _propAccessor(entidade);
            return !string.IsNullOrWhiteSpace(email) && ValidarEmail(email);
        }

        /// <summary>
        /// Realiza a validação do e-mail com base em uma expressão regular.
        /// </summary>
        /// <param name="email">Endereço de e-mail a ser validado.</param>
        /// <returns>True se o e-mail for válido; caso contrário, false.</returns>
        private bool ValidarEmail(string email)
        {
            var rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            return rg.IsMatch(email);
        }
    }
}
