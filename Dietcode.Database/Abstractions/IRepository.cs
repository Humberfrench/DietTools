namespace Dietcode.Database.Abstractions
{
    public interface IRepository<T> :
        IReadRepository<T>,
        IWriteRepository<T>
        where T : class
    {
    }
}
