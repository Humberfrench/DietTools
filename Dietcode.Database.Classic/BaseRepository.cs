using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;


namespace Dietcode.Database.Classic
{
    public class BaseRepository<T> : Database<T> where T : class
    {
        public string ConnectionString { get; set; }
        public string Erro//for dapper
        public IDbConnection Connection { get; set; }
        public bool SaveAuto { get; set; }

        public BaseRepository() : base()
        {
            ConnectionString = "";
            Erro = "";
            Connection = new SqlConnection();
            SaveAuto = false;
        }
        public BaseRepository(string connectionString) : base(connectionString)
        {
            Erro = "";
            ConnectionString = connectionString;
            Connection = new SqlConnection(ConnectionString);
            SaveAuto = false;
        }

        public void ReNewConn()
        {
            Connection = new SqlConnection(ConnectionString);
        }

        //TODO => Get id
        public async virtual Task<int> Adicionar(T dataEntity)
        {
            var id = 0;
            await this.AddAsync(dataEntity);
            if(SaveAuto)
            {
                try
                {
                    id = SaveChanges();
                }
                catch (Exception ex)
                {
                    Erro += ex.Message;
                    if(ex.InnerException !=null)
                    {
                        var inner = ex.InnerException;
                        Erro += $" - {inner.Message}";
                        if (inner.InnerException != null)
                        {
                            var inner2 = ex.InnerException;
                            Erro += $" - {inner2.Message}";
                            if (inner2.InnerException != null)
                            {
                                var inner3 = ex.InnerException;
                                Erro += $" - {inner3.Message}";
                            }
                        }
                    }
                    throw;
                }
            }
            return id;
        }

        public async virtual Task<bool> Atualizar(T dataEntity)
        {
            await Task.Run(() => this.Update(dataEntity));
            return true;
        }

        public async virtual Task<bool> Remover(T dataEntity)
        {
            await Task.Run(() => this.Remove(dataEntity));
            return true;
        }

        public async Task<List<T>> LoadAll()
        {
            var query = this.Set<T>().AsQueryable().AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<T?> Get(int id)
        {
            var query = await this.Set<T>().FindAsync(id);
            return query;
        }

        public async Task<List<T>> Get(Expression<Func<T, bool>> predicate)
        {
            var query = this.Set<T>().Where(predicate).AsQueryable().AsNoTracking();
            return await query.ToListAsync();
        }

    }
}
