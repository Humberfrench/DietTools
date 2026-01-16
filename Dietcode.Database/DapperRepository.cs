using Dapper;
using Dapper.Contrib.Extensions;
using Dietcode.Database.Abstractions;
using System.Data;
using System.Data.Common;

namespace Dietcode.Database
{
    public sealed class DapperRepository<T>(IConnectionFactory factory) : IRepository<T>
        where T : class
    {
        private readonly IConnectionFactory _factory = factory;

        private async Task<IDbConnection> OpenAsync(
            CancellationToken cancellationToken)
        {
            var conn = _factory.Create();

            if (conn is DbConnection db)
                await db.OpenAsync(cancellationToken);
            else
                conn.Open();

            return conn;
        }

        // -------------------------
        // READ
        // -------------------------

        public async Task<T?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            using var conn = await OpenAsync(cancellationToken);
            return await conn.GetAsync<T>(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            using var conn = await OpenAsync(cancellationToken);
            var result = await conn.GetAllAsync<T>();
            return result.AsList();
        }

        public async Task<IReadOnlyList<T>> QueryAsync(
            string sql,
            object? param = null,
            CancellationToken cancellationToken = default)
        {
            using var conn = await OpenAsync(cancellationToken);
            var result = await conn.QueryAsync<T>(sql, param);
            return result.AsList();
        }

        // -------------------------
        // WRITE
        // -------------------------

        public async Task<long> InsertAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            using var conn = await OpenAsync(cancellationToken);
            return await conn.InsertAsync(entity);
        }

        public async Task<bool> UpdateAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            using var conn = await OpenAsync(cancellationToken);
            return await conn.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            using var conn = await OpenAsync(cancellationToken);
            var entity = await conn.GetAsync<T>(id);
            return entity != null && await conn.DeleteAsync(entity);
        }
    }
}
