using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Storage;
using Dapper;

namespace Dietcode.Database.Classic
{
    public class BaseRepository<T> where T : class //: Database<T>
    {
        public string ConnectionString { get; set; }
        public string Erro { get; set; }
        public IDbConnection Connection { get; set; }
        public bool SaveAuto { get; set; }

        private readonly Database<T> db;
        public BaseRepository() //: base()
        {
            ConnectionString = "";
            Erro = "";
            Connection = new SqlConnection();
            SaveAuto = false;
            db = new Database<T>();
        }
        public BaseRepository(string connectionString) //: base(connectionString)
        {
            Erro = "";
            ConnectionString = connectionString;
            Connection = new SqlConnection(ConnectionString);
            SaveAuto = false;
            db = new Database<T>(connectionString);
        }

        public void ReNewConn()
        {
            Connection = new SqlConnection(ConnectionString);
        }

        //TODO => Get id
        public async virtual Task<int> Adicionar(T dataEntity)
        {
            var id = 0;
            await db.AddAsync(dataEntity);
            if(SaveAuto)
            {
                try
                {
                    id = await SaveChangesAsync();
                    var entries = db.ChangeTracker.Entries;
                    
                }
                catch (Exception ex)
                {
                    Erro = GetErro(ex);
                }
            }
            return id;
        }

        //chamar comando sql
        public async Task<List<T>> Query(string sql)
        {
            IEnumerable<T> result = new List<T>();
            try
            {
                result = await Connection.QueryAsync<T>(sql);
            }
            catch (Exception ex)
            {
                Erro = GetErro(ex);
            }
            return result.ToList();
        }

        public async Task<IDbContextTransaction> BeginTransaction() => await db.Database.BeginTransactionAsync();
        public async Task Commit() => await db.Database.CommitTransactionAsync();
        public async Task RoolBack() => await db.Database.RollbackTransactionAsync();

        public async virtual Task<bool> Atualizar(T dataEntity)
        {
            await Task.Run(() => db.Update(dataEntity));
            return true;
        }

        public async virtual Task<bool> Remover(T dataEntity)
        {
            await Task.Run(() => db.Remove(dataEntity));
            return true;
        }

        public async Task<List<T>> LoadAll()
        {
            var query = db.Set<T>().AsQueryable().AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<T?> Get(int id)
        {
            var query = await db.Set<T>().FindAsync(id);
            return query;
        }

        public async Task<List<T>> Get(Expression<Func<T, bool>> predicate)
        {
            var query = db.Set<T>().Where(predicate).AsQueryable().AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Erro = GetErro(ex);
                return 0;
            }
        }
        string GetErro(Exception ex)
        {
            var erro = string.Empty;
            erro += ex.Message;
            if (ex.InnerException != null)
            {
                var inner = ex.InnerException;
                erro += $" - {inner.Message}";
                if (inner.InnerException != null)
                {
                    var inner2 = ex.InnerException;
                    erro += $" - {inner2.Message}";
                    if (inner2.InnerException != null)
                    {
                        var inner3 = ex.InnerException;
                        erro += $" - {inner3.Message}";
                    }
                }
            }

            return erro;
        }
    }
}
