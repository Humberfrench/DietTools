using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Dietcode.Database.Enums;
using Dietcode.Database.Interfaces;
using Dietcode.Database.DatabaseProviders;

namespace Dietcode.Database
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly string ConnectionString;
        protected readonly IDbConnection Connection;

        public Repository(string connectionString, EnumBancos banco) : base()
        {
            ConnectionString = connectionString;
            IConnectionFactory connectionFactory;

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
            Connection = connectionFactory.Connection();
        }

        public async Task<int> Add(T entity)
        {
            return await Connection.InsertAsync<T>(entity);
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await Connection.GetAsync<T>(id);
            if(entity == null)
            {
                return false;
            }
            return await Connection.DeleteAsync<T>(entity);
        }

        public async Task<T> Get(int id)
        {
            return await Connection.GetAsync<T>(id);
        }

        public async Task<IEnumerable<T>> Get()
        {
            return await Connection.GetAllAsync<T>();
        }

        public async Task<bool> Update(T entity)
        {
            return await Connection.UpdateAsync<T>(entity);
        }
    }
}
