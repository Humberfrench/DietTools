using Dietcode.Core.DomainValidator;

namespace Dietcode.Core.DomainValidator.Interfaces
{
    public interface IValidator<in TEntity>
    {
        ValidationResult Validar(TEntity entity);
    }
}
