using Dietcode.Database.Interfaces;
using MySqlConnector;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public class MySqlConnectionFactory : IConnectionFactory
    {
        protected readonly string ConnectionString;

        public MySqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection Connection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
