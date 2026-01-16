using Dietcode.Database.Abstractions;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public sealed class OracleConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public OracleConnectionFactory(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection Create()
        {
            return new OracleConnection(_connectionString);
        }
    }
}
