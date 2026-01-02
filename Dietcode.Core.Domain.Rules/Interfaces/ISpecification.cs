namespace Dietcode.Core.Domain.Rules.Interfaces
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T entidade);
    }
}
