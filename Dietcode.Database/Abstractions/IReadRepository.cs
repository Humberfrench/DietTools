namespace Dietcode.Database.Abstractions
{
    public interface IReadRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> QueryAsync(
            string sql,
            object? param = null,
            CancellationToken cancellationToken = default);
    }
}
