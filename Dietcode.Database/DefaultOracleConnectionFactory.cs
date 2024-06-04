using Microsoft.Data.SqlClient;
using System.Data;

namespace Dietcode.Database
{
    public class DefaultOracleConnectionFactory : IConnectionFactory
    {
        protected readonly string ConnectionString;

        public DefaultOracleConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection Connection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}