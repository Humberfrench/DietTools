using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Linq.Expressions;

namespace French.Erp.Domain.Specifications
{

    public class PropriedadeDiaValido<T> : ISpecification<T>
    {
        private readonly Func<T, int> _diaAccessor;
        private readonly Func<T, int> _mesAccessor;

        public PropriedadeDiaValido(Expression<Func<T, int>> dia, Expression<Func<T, int>> mes)
        {
            _diaAccessor = dia.Compile();
            _mesAccessor = mes.Compile();
        }

        public bool IsSatisfiedBy(T entidade)
        {
            int dia = _diaAccessor(entidade);
            int mes = _mesAccessor(entidade);

            switch (mes)
            {
                case 1: // Janeiro
                case 3: // Março
                case 5: // Maio
                case 7: // Julho
                case 8: // Agosto
                case 10: // Outubro
                case 12: // Dezembro
                    return dia >= 1 && dia <= 31;
                case 4: // Abril
                case 6: // Junho
                case 9: // Setembro
                case 11: // Novembro
                    return dia >= 1 && dia <= 30;
                case 2: // Fevereiro
                    return dia >= 1 && dia <= 28; // Não considera anos bissextos
                default:
                    return false; // Mês inválido
            }
        }
    }
}
