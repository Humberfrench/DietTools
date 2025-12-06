using Dapper;
using Dapper.Contrib.Extensions;
using Dietcode.Database.Enums;
using Dietcode.Database.Interfaces;
using System.Data;

namespace Dietcode.Database
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IConnectionFactory _factory;

        public Repository(string connectionString, EnumBancos banco)
        {
            _factory = ConnectionFactorySelector.Create(connectionString, banco);
        }

        private IDbConnection GetConnection()
        {
            var conn = _factory.Connection();
            if (conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        }

        // 🔹 Transação opcional
        public IDbTransaction BeginTransaction(out IDbConnection connection)
        {
            connection = GetConnection();
            return connection.BeginTransaction();
        }

        #region Async CRUD

        public async Task<int> AddAsync(T entity)
        {
            using var conn = GetConnection();
            return await conn.InsertAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = GetConnection();
            var obj = await conn.GetAsync<T>(id);
            return obj != null && await conn.DeleteAsync(obj);
        }

        public async Task<T> GetAsync(int id)
        {
            using var conn = GetConnection();
            return await conn.GetAsync<T>(id);
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            using var conn = GetConnection();
            return await conn.GetAllAsync<T>();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            using var conn = GetConnection();
            return await conn.UpdateAsync(entity);
        }

        public async Task<IEnumerable<T>> CustomQueryAsync(Func<T, bool> predicate)
        {
            using var conn = GetConnection();
            return (await conn.GetAllAsync<T>())
                .Where(predicate);
        }

        #endregion

        #region Sync CRUD

        public long Add(T entity)
        {
            using var conn = GetConnection();
            return conn.Insert(entity);
        }

        public bool Delete(int id)
        {
            using var conn = GetConnection();
            var obj = conn.Get<T>(id);
            return obj != null && conn.Delete(obj);
        }

        public T Get(int id)
        {
            using var conn = GetConnection();
            return conn.Get<T>(id);
        }

        public IEnumerable<T> Get()
        {
            using var conn = GetConnection();
            return conn.GetAll<T>();
        }

        public bool Update(T entity)
        {
            using var conn = GetConnection();
            return conn.Update(entity);
        }

        public IEnumerable<T> QueryCustom(Func<T, bool> predicate)
        {
            using var conn = GetConnection();
            return conn.GetAll<T>().Where(predicate);
        }

        #endregion

        // 🔹 Nova API SQL Direto (não quebra compatibilidade)
        public Task<IEnumerable<T>> QuerySqlAsync(string sql, object? param = null)
        {
            using var conn = GetConnection();
            return conn.QueryAsync<T>(sql, param);
        }
    }
}
