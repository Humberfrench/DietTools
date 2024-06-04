using Dietcode.Database.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
