using Dietcode.Database.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        protected readonly string ConnectionString;

        public SqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection Connection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
