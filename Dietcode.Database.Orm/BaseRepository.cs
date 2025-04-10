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
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        protected DbSet<TEntity> DbSet;
        protected readonly ThisDatabase<TEntity> Context;
        private readonly IMyContextManager<ThisDatabase<TEntity>> contextManager;

        public BaseRepository(IMyContextManager<ThisDatabase<TEntity>> contextManager)
        {
            this.contextManager = contextManager;
            Context = contextManager.GetContext();
            DbSet = Context.Set<TEntity>();
        }

        //for dapper
        public IDbConnection Connection => new SqlConnection(Context.ConnectionString);

        public async virtual Task<bool> Adicionar(TEntity obj)
        {
            var entry = Context.Entry(obj);
            await DbSet.AddAsync(obj);
            entry.State = EntityState.Added;
            return true;
        }

        public async virtual Task<bool> Atualizar(TEntity obj)
        {
            var entry = Context.Entry(obj);
            await Task.Run(() => DbSet.Attach(obj));
            entry.State = EntityState.Modified;
            return true;
        }

        public async virtual Task<bool> Remover(TEntity obj)
        {
            var entry = Context.Entry(obj);
            await Task.Run(() => DbSet.Remove(obj));
            //DbSet.Remove(obj);
            entry.State = EntityState.Deleted;

            return true;
        }

        public async virtual Task<TEntity> ObterPorId(int id)
        {
            var resultado = await DbSet.FindAsync(id);
            return resultado ?? new TEntity();
        }

        public async virtual Task<IEnumerable<TEntity>> ObterTodos()
        {
            return await Task.Run(() => DbSet.ToList());
        }

        public async virtual Task<IEnumerable<TEntity>> ObterTodosPaginado(int pagina, int registros)
        {
            return await Task.Run(() => DbSet.Take(pagina).Skip(registros));
        }

        public async virtual Task<IEnumerable<TEntity>> Pesquisar(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.Run(() => DbSet.Where(predicate));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}
