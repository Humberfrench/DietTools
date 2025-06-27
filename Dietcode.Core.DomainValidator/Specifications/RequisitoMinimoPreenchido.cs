using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace French.Erp.Domain.Specifications
{
    public class RequisitoMinimoPreenchido<T> : ISpecification<T>
    {
        private readonly Func<T, string>[] _propriedades;
        public RequisitoMinimoPreenchido(params Expression<Func<T, string>>[] propriedades)
        {
            _propriedades = propriedades.Select(p => p.Compile()).ToArray();
        }
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
