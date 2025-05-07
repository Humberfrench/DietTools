using Dietcode.Core.DomainValidator;
using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Dietcode.Database.Orm.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace Dietcode.Database.Orm
{
    public abstract class Repository<Table> : IBaseRepository<Table> where Table : class, new()
    {
        protected DbSet<Table> DbSet;
        protected readonly ThisDatabase<Table> Context;
        private readonly IMyContextManager<ThisDatabase<Table>> contextManager;
        private readonly IMyUnitOfWork<Table> myUnitOfWork;

        public Repository(IMyContextManager<ThisDatabase<Table>> contextManager)
        {
            this.contextManager = contextManager;
            Context = contextManager.GetContext();
            DbSet = Context.Set<Table>();
            this.myUnitOfWork = new MyUnitOfWork<Table>(contextManager);
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

        #region Uow
        public void BeginTransaction()
        {
            myUnitOfWork.BeginTransaction();
        }

        public ValidationResult<Table> SaveChanges()
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

