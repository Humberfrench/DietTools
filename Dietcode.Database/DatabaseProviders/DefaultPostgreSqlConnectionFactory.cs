using Dietcode.Database.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public class DefaultPostgreSqlConnectionFactory : IConnectionFactory
    {
        protected readonly string ConnectionString;

        public DefaultPostgreSqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection Connection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}