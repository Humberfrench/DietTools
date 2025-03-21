using Microsoft.Data.SqlClient;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public class DefaultMySqlConnectionFactory : IConnectionFactory
    {
        protected readonly string ConnectionString;

        public DefaultMySqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection Connection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
