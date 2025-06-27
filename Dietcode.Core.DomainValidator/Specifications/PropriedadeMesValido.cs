using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Linq.Expressions;

namespace French.Erp.Domain.Specifications
{

    public class PropriedadeMesValido<T> : ISpecification<T>
    {
        private readonly Func<T, int> _mesAccessor;

        public PropriedadeMesValido(Expression<Func<T, int>> mes)
        {
            _mesAccessor = mes.Compile();
        }

        public bool IsSatisfiedBy(T entidade)
        {
            int mes = _mesAccessor(entidade);
            return mes >= 1 && mes <= 12;
        }
    }
}
