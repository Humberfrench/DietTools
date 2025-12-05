using Dietcode.Database.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public class DefaultSqlConnectionFactory : IConnectionFactory
    {
        protected readonly string ConnectionString;

        public DefaultSqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection Connection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
