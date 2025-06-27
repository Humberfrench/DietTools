using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Linq.Expressions;

namespace French.Erp.Domain.Specifications
{

    public class PropriedadeIntMaiorIgualZero<T> : ISpecification<T>
    {
        private readonly Func<T, int> _propAccessor;
        public PropriedadeIntMaiorIgualZero(Expression<Func<T, int>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }
        public bool IsSatisfiedBy(T entidade)
        {
            var valor = _propAccessor(entidade);
            return valor >= 0;
        }
    }
}