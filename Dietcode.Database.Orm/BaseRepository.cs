using Dietcode.Core.DomainValidator;
using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Dietcode.Database.Orm.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

////MUST Add
//{
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
        private readonly IMyUnitOfWork<Table> myUnitOfWork;

        public BaseRepository(IMyContextManager<ThisDatabase<Table>> contextManager)
        {
            this.contextManager = contextManager;
            Context = contextManager.GetContext();
            this.myUnitOfWork = new MyUnitOfWork<Table>(Context);
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

            var entries = Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();

            return entries.Count > 0;
        }

        public async virtual Task<bool> Atualizar(Table obj)
        {
            var entry = Context.Entry(obj);
            await Task.Run(() => DbSet.Attach(obj));
            //await Task.Run(() => DbSet.Update(obj));
            entry.State = EntityState.Modified;

            var entries = Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();

            return entries.Count > 0;

        }

        public async virtual Task<bool> Remover(Table obj)
        {
            var entry = Context.Entry(obj);
            await Task.Run(() => DbSet.Remove(obj));
            //DbSet.Remove(obj);
            entry.State = EntityState.Deleted;

            var entries = Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList();

            return entries.Count > 0;
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

        #region Uow
        public void BeginTransaction()
        {
            myUnitOfWork.BeginTransaction();
        }

        public ValidationResult<Table> Commit()
        {
            return myUnitOfWork.SaveChanges();
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

