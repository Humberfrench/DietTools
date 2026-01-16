using Dietcode.Database.Abstractions;


namespace Dietcode.Database.Logging
{
    public abstract class RepositoryDecorator<T> : IRepository<T>
        where T : class
    {
        protected readonly IRepository<T> Inner;

        protected RepositoryDecorator(IRepository<T> inner)
        {
            Inner = inner;
        }

        // -------- READ --------

        public virtual Task<T?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
            => Inner.GetByIdAsync(id, cancellationToken);

        public virtual Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken cancellationToken = default)
            => Inner.GetAllAsync(cancellationToken);

        public virtual Task<IReadOnlyList<T>> QueryAsync(
            string sql,
            object? param = null,
            CancellationToken cancellationToken = default)
            => Inner.QueryAsync(sql, param, cancellationToken);

        // -------- WRITE --------

        public virtual Task<long> InsertAsync(
            T entity,
            CancellationToken cancellationToken = default)
            => Inner.InsertAsync(entity, cancellationToken);

        public virtual Task<bool> UpdateAsync(
            T entity,
            CancellationToken cancellationToken = default)
            => Inner.UpdateAsync(entity, cancellationToken);

        public virtual Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
            => Inner.DeleteAsync(id, cancellationToken);
    }
}
