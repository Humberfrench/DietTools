using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

////MUST Add
//{
//    services.AddScoped(typeof(IMyUnitOfWork<>), typeof(MyUnitOfWork<>));
//    services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
//    services.AddScoped(typeof(IMyContextManager<>), typeof(MyContextManager<>));

//}

namespace Dietcode.Database.Orm
{
    public class BaseRepository<Table> : IBaseRepository<Table> where Table : class, new()
    {
        protected DbSet<Table> DbSet;
        protected readonly ThisDatabase<Table> Context ;
        private readonly IMyContextManager<ThisDatabase<Table>> contextManager;

        public BaseRepository(IMyContextManager<ThisDatabase<Table>> contextManager)
        {
            this.contextManager = contextManager;
            Context = contextManager.GetContext();
            DbSet = Context.Set<Table>();
        }

        #region Dapper

        //for dapper
        public IDbConnection Connection => new SqlConnection(Context.ConnectionString);

        #endregion

        #region Crud

        public async virtual Task<bool> Adicionar(Table obj)
        {
            var entry = Context.Entry(obj);
            await DbSet.AddAsync(obj);
            entry.State = EntityState.Added;
            return true;
        }

        public async virtual Task<bool> Atualizar(Table obj)
        {
            var entry = Context.Entry(obj);
            await Task.Run(() => DbSet.Attach(obj));
            entry.State = EntityState.Modified;
            return true;
        }

        public async virtual Task<bool> Remover(Table obj)
        {
            var entry = Context.Entry(obj);
            await Task.Run(() => DbSet.Remove(obj));
            //DbSet.Remove(obj);
            entry.State = EntityState.Deleted;

            return true;
        }

        public async virtual Task<Table> ObterPorId(int id)
        {
            var resultado = await DbSet.FindAsync(id);
            return resultado ?? new Table();
        }

        public async virtual Task<IEnumerable<Table>> ObterTodos()
        {
            return await Task.Run(() => DbSet.ToList());
        }

        public async virtual Task<IEnumerable<Table>> ObterTodosPaginado(int pagina, int registros)
        {
            return await Task.Run(() => DbSet.Take(pagina).Skip(registros));
        }

        public async virtual Task<IEnumerable<Table>> Pesquisar(Expression<Func<Table, bool>> predicate)
        {
            return await Task.Run(() => DbSet.Where(predicate));
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}

