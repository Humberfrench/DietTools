namespace Dietcode.Classic.Domain.Rules.Interfaces
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T entidade);
    }
}

