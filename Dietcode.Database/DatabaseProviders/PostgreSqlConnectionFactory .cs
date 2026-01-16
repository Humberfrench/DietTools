using Dietcode.Database.Abstractions;
using Npgsql;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public sealed class PostgreSqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public PostgreSqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection Create()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
