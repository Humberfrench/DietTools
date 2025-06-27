using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace French.Erp.Domain.Specifications
{
    public class PropriedadeEmailValido<T> : ISpecification<T>
    {
        private readonly Func<T, string> _propAccessor;
        public PropriedadeEmailValido(Expression<Func<T, string>> propriedade)
        {
            _propAccessor = propriedade.Compile();
        }
        public bool IsSatisfiedBy(T entidade)
        {
            var email = _propAccessor(entidade);
            return !string.IsNullOrWhiteSpace(email) && ValidarEmail(email);
        }

        bool ValidarEmail(string email)
        {
            var rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

            return rg.IsMatch(email);
        }

    }
}
