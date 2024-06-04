using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly string ConnectionString;
        protected readonly IConnectionFactory connectionFactory;

        public Repository(string connectionString, EnumBancos banco) : base()
        {
            ConnectionString = connectionString;
            switch (banco)
            {
                case EnumBancos.SqlServer:
                    connectionFactory = new DefaultSqlConnectionFactory(ConnectionString);
                    break;
                case EnumBancos.MySql:
                    connectionFactory = new DefaultMySqlConnectionFactory(ConnectionString);
                    break;
                case EnumBancos.PostgreSql:
                    connectionFactory = new DefaultPostgreSqlConnectionFactory(ConnectionString);
                    break;
                case EnumBancos.Oracle:
                    connectionFactory = new DefaultOracleConnectionFactory(ConnectionString);
                    break;
                default:
                    connectionFactory = new DefaultSqlConnectionFactory(ConnectionString);
                    break;
            }
        }

        public Task<T> Add(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<T> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<T> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<T> Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
