using Dietcode.Database.Interfaces;
using Npgsql;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public class PostgreSqlConnectionFactory : IConnectionFactory
    {
        protected readonly string ConnectionString;

        public PostgreSqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection Connection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}