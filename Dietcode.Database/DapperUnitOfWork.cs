using Dietcode.Database.Abstractions;
using System.Data;
using System.Data.Common;

namespace Dietcode.Database
{
    public sealed class DapperUnitOfWork
    {
        private readonly IConnectionFactory _factory;

        public DapperUnitOfWork(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task ExecuteAsync(
            Func<IDbConnection, IDbTransaction, Task> action,
            CancellationToken cancellationToken = default)
        {
            using var conn = _factory.Create();

            if (conn is DbConnection db)
                await db.OpenAsync(cancellationToken);
            else
                conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                await action(conn, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
