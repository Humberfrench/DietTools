using Dietcode.Core.Domain.Rules.Interfaces;
using System.Linq.Expressions;

namespace Dietcode.Core.Domain.Rules.Specifications
{
    /// <summary>
    /// Especificação que valida se a combinação de dia e mês representa uma data válida (sem considerar ano bissexto).
    /// </summary>
    /// <typeparam name="T">Tipo da entidade a ser validada.</typeparam>
    public class PropriedadeDiaValido<T> : ISpecification<T>
    {
        private readonly Func<T, int> _diaAccessor;
        private readonly Func<T, int> _mesAccessor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PropriedadeDiaValido{T}"/>.
        /// </summary>
        /// <param name="dia">Expressão lambda que representa a propriedade do dia.</param>
        /// <param name="mes">Expressão lambda que representa a propriedade do mês.</param>
        public PropriedadeDiaValido(Expression<Func<T, int>> dia, Expression<Func<T, int>> mes)
        {
            _diaAccessor = dia.Compile();
            _mesAccessor = mes.Compile();
        }

        /// <summary>
        /// Verifica se o valor do dia é válido para o mês fornecido.
        /// </summary>
        /// <param name="entidade">Instância da entidade a ser validada.</param>
        /// <returns>
        /// True se o dia for válido para o mês; caso contrário, false.
        /// </returns>
        public bool IsSatisfiedBy(T entidade)
        {
            int dia = _diaAccessor(entidade);
            int mes = _mesAccessor(entidade);

            switch (mes)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return dia >= 1 && dia <= 31;
                case 4:
                case 6:
                case 9:
                case 11:
                    return dia >= 1 && dia <= 30;
                case 2:
                    return dia >= 1 && dia <= 28; // Não considera anos bissextos
                default:
                    return false; // Mês inválido
            }
        }
    }
}
