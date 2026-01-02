namespace Dietcode.Core.Domain.Rules.Interfaces
{
    public interface IValidator<in TEntity>
    {
        ValidatorRules Validar(TEntity entity);
    }
}
