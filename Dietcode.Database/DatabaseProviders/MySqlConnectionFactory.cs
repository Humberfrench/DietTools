using Dietcode.Database.Abstractions;
using MySqlConnector;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public sealed class MySqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection Create()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
