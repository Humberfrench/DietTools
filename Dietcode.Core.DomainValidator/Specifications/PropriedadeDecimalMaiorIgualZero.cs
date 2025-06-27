using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Linq.Expressions;

namespace French.Erp.Domain.Specifications
{

    public class PropriedadeDecimalMaiorQueZero<T> : ISpecification<T>
    {
        private readonly Func<T, decimal> _propAccessor;
        public PropriedadeDecimalMaiorQueZero(Expression<Func<T, decimal>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }
        public bool IsSatisfiedBy(T entidade)
        {
            var valor = _propAccessor(entidade);
            return valor > 0m;
        }
    }
}