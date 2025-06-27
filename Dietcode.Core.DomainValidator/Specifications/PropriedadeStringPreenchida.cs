using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Linq.Expressions;

namespace French.Erp.Domain.Specifications
{

    public class PropriedadeStringPreenchida<T> : ISpecification<T>
    {
        private readonly Func<T, string> _propAccessor;

        public PropriedadeStringPreenchida(Expression<Func<T, string>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }

        public bool IsSatisfiedBy(T entidade)
        {
            var valor = _propAccessor(entidade);
            return !string.IsNullOrWhiteSpace(valor);
        }
    }
}
