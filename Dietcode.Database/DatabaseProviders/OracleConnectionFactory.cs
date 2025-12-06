using Dietcode.Database.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Dietcode.Database.DatabaseProviders
{
    public class OracleConnectionFactory : IConnectionFactory
    {
        protected readonly string ConnectionString;

        public OracleConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection Connection()
        {
            return new OracleConnection(ConnectionString);
        }
    }
}