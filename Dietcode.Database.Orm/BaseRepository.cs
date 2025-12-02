using Dietcode.Core.DomainValidator;
using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Dietcode.Database.Orm.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dietcode.Database.Orm.Logging;

namespace Dietcode.Database.Orm
{
    public class BaseRepository<Table, Tipo> : IBaseRepository<Table, Tipo> where Table : class, new()
    {
        protected readonly ThisDatabase<Table> Context;
        protected readonly DbSet<Table> DbSet;
        private readonly IMyContextManager<ThisDatabase<Table>> contextManager;
        private readonly IMyUnitOfWork<Table> myUnitOfWork;
        private ILogger Logger => InternalOrmLoggerFactory.Instance.CreateLogger<BaseRepository<Table, Tipo>>();

        public BaseRepository(IMyContextManager<ThisDatabase<Table>> contextManager)
        {
            this.contextManager = contextManager;
            Context = contextManager.GetContext();
            myUnitOfWork = new MyUnitOfWork<Table>(Context);
            DbSet = Context.Set<Table>();
        }

        #region Dapper

        public IDbConnection Connection => new SqlConnection(Context.ConnectionString);

        #endregion

        #region Crud

        public virtual async Task<bool> Adicionar(Table obj)
        {
            await DbSet.AddAsync(obj);
            Logger.LogInformation("Inserindo entidade {Entity}", typeof(Table).Name);
            Logger.LogDebug("Inserindo entidade {Entity}", typeof(Table).Name);

            var entries = Context.ChangeTracker.Entries()
                .Where(e => e.Entity == obj && e.State == EntityState.Added)
                .ToList();

            return entries.Count > 0;
        }

        public virtual Task<bool> Atualizar(Table obj)
        {
            var entry = Context.Entry(obj);
            entry.State = EntityState.Modified;
            Logger.LogInformation("Atualizando entidade {Entity}", typeof(Table).Name);
            Logger.LogDebug("Atualizando entidade {Entity}", typeof(Table).Name);

            var entries = Context.ChangeTracker.Entries()
                .Where(e => e.Entity == obj && e.State == EntityState.Modified)
                .ToList();

            return Task.FromResult(entries.Count > 0);
        }

        public virtual Task<bool> Remover(Table obj)
        {
            var entry = Context.Entry(obj);
            entry.State = EntityState.Deleted;
            Logger.LogInformation("Removendo entidade {Entity}", typeof(Table).Name);
            Logger.LogDebug("Removendo entidade {Entity}", typeof(Table).Name);

            var entries = Context.ChangeTracker.Entries()
                .Where(e => e.Entity == obj && e.State == EntityState.Deleted)
                .ToList();

            return Task.FromResult(entries.Count > 0);
        }

        public virtual async Task<Table> ObterPorId(Tipo id)
        {
            var resultado = await DbSet.FindAsync(id);
            return resultado ?? new Table();
        }

        public virtual async Task<IEnumerable<Table>> ObterTodos()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<Table>> ObterTodosPaginado(int pagina, int registros)
        {
            if (pagina <= 0) pagina = 1;
            if (registros <= 0) registros = 10;

            return await DbSet
                .Skip((pagina - 1) * registros)
                .Take(registros)
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<Table>> Pesquisar(Expression<Func<Table, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
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
            // Context é gerenciado via DI (Scoped), então não damos Dispose aqui.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
