namespace Dietcode.Database.Abstractions
{
    public interface IWriteRepository<T> where T : class
    {
        Task<long> InsertAsync(
            T entity,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            T entity,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
