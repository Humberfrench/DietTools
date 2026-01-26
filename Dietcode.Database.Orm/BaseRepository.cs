using System.Data;
using System.Linq.Expressions;
using Dietcode.Core.DomainValidator;
using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Dietcode.Database.Orm.Logging;
using Dietcode.Database.Orm.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public virtual async Task<bool> Adicionar(Table obj, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await DbSet.AddAsync(obj);
            Logger.LogInformation("Inserindo entidade {Entity}", typeof(Table).Name);
            Logger.LogDebug("Inserindo entidade {Entity}", typeof(Table).Name);

            var entries = Context.ChangeTracker.Entries()
                .Where(e => e.Entity == obj && e.State == EntityState.Added)
                .ToList();

            return entries.Count > 0;
        }

        public virtual Task<bool> Atualizar(Table obj, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var entry = Context.Entry(obj);
            entry.State = EntityState.Modified;
            Logger.LogInformation("Atualizando entidade {Entity}", typeof(Table).Name);
            Logger.LogDebug("Atualizando entidade {Entity}", typeof(Table).Name);

            var entries = Context.ChangeTracker.Entries()
                .Where(e => e.Entity == obj && e.State == EntityState.Modified)
                .ToList();

            return Task.FromResult(entries.Count > 0);
        }

        public virtual Task<bool> Remover(Table obj, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var entry = Context.Entry(obj);
            entry.State = EntityState.Deleted;
            Logger.LogInformation("Removendo entidade {Entity}", typeof(Table).Name);
            Logger.LogDebug("Removendo entidade {Entity}", typeof(Table).Name);

            var entries = Context.ChangeTracker.Entries()
                .Where(e => e.Entity == obj && e.State == EntityState.Deleted)
                .ToList();

            return Task.FromResult(entries.Count > 0);
        }

        public virtual async Task<Table> ObterPorId(Tipo id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            // FindAsync com ct: usar overload com object[]
            var resultado = await DbSet.FindAsync(new object[] { id! }, ct);
            return resultado ?? new Table();
        }

        public virtual async Task<IEnumerable<Table>> ObterTodos(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await DbSet.ToListAsync(ct);
        }

        public virtual async Task<IEnumerable<Table>> ObterTodos(int pagina, int registros, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (pagina <= 0) pagina = 1;
            if (registros <= 0) registros = 10;


            return await DbSet
            .Skip((pagina - 1) * registros)
            .Take(registros)
            .ToListAsync(ct);
        }
        public virtual async Task<IEnumerable<Table>> Pesquisar(Expression<Func<Table, bool>> predicate, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await DbSet.Where(predicate).ToListAsync(ct);
        }

        #endregion

        #region Uow

        public void BeginTransaction()
        {
            myUnitOfWork.BeginTransaction();
        }

        public async Task<ValidationResult<Table>> Commit(CancellationToken ct = default)
        {
            return await myUnitOfWork.SaveChanges(ct);
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
