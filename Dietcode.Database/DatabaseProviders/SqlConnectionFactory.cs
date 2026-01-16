using Dietcode.Database.Abstractions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public sealed class SqlServerConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public SqlServerConnectionFactory(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
