using Dietcode.Core.DomainValidator.Interfaces;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace French.Erp.Domain.Specifications
{
    public class PropriedadeNumeroStringValido<T> : ISpecification<T>
    {
        private readonly Func<T, string> _stringAccessor;
        public PropriedadeNumeroStringValido(Expression<Func<T, string>> propriedade)
        {
            _stringAccessor = propriedade.Compile();
        }
        public bool IsSatisfiedBy(T entidade)
        {
            string valor = _stringAccessor(entidade);

            // Verifica se a string não é nula ou vazia e se contém apenas dígitos
            return !string.IsNullOrWhiteSpace(valor) && Regex.IsMatch(valor, @"^\d+$");
        }
    }
}
