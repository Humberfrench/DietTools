using Dapper.Contrib.Extensions;
using System.Data;

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

        #region Async
        public async Task<int> AddAsync(T entity)
        {
            return await Connection.InsertAsync<T>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await Connection.GetAsync<T>(id);
            if (entity == null)
            {
                return false;
            }
            return await Connection.DeleteAsync<T>(entity);
        }

        public async Task<T> GetAsync(int id)
        {
            return await Connection.GetAsync<T>(id);
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            return await Connection.GetAllAsync<T>();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            return await Connection.UpdateAsync<T>(entity);
        }

        public virtual async Task<IEnumerable<T>> CustomQueryAsync(Func<T, bool> predicate)
        {
            return (await this.Connection.GetAllAsync<T>()).Where(predicate);
        }

        #endregion    

        #region Sync
        public long Add(T entity)
        {
            return Connection.Insert<T>(entity);
        }

        public bool Delete(int id)
        {
            var entity = Connection.Get<T>(id);
            if (entity == null)
            {
                return false;
            }
            return Connection.Delete<T>(entity);
        }

        public T Get(int id)
        {
            return Connection.Get<T>(id);
        }

        public IEnumerable<T> Get()
        {
            return Connection.GetAll<T>();
        }

        public bool Update(T entity)
        {
            return Connection.Update<T>(entity);
        }

        public virtual IEnumerable<T> QueryCustom(Func<T, bool> predicate)
        {
            return this.Connection.GetAll<T>().Where(predicate);
        }

        #endregion

    }

}
