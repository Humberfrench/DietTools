using System.Data;
using System.Linq.Expressions;
using Dapper;
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
        private readonly string connectionString;
        protected ILogger Logger => InternalOrmLoggerFactory.Instance.CreateLogger<BaseRepository<Table, Tipo>>();

        public BaseRepository(IMyContextManager<ThisDatabase<Table>> contextManager)
        {
            this.contextManager = contextManager;
            Context = contextManager.GetContext();
            myUnitOfWork = new MyUnitOfWork<Table>(Context);
            DbSet = Context.Set<Table>();
            connectionString = Context.ConnectionString;
        }

        #region Dapper

        public IDbConnection Connection => new SqlConnection(connectionString);

        #endregion

        #region Crud

        public virtual async Task<bool> Adicionar(Table obj, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            await DbSet.AddAsync(obj, ct);
            Logger.LogInformation("Inserindo entidade {Entity}", typeof(Table).Name);
            Logger.LogDebug("Inserindo entidade {Entity}", typeof(Table).Name);

            var entries = Context.ChangeTracker.Entries()
                .Where(e => e.Entity == obj && e.State == EntityState.Added)
                .ToList();

            return entries.Count > 0;
        }

        public virtual Task<bool> Atualizar(Table obj, CancellationToken ct = default)
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

        public virtual Task<bool> Remover(Table obj, CancellationToken ct = default)
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

        public virtual async Task<Table?> ObterPorId(Tipo id, bool asTracking = true, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var entity = await DbSet.FindAsync(new object[] { id! }, ct);
            if (entity is null)
                return null;

            if (!asTracking)
                Context.Entry(entity).State = EntityState.Detached;

            return entity;
        }

        public virtual Task<List<Table>> ObterTodos(bool asTracking = false, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            IQueryable<Table> query = DbSet;
            if (!asTracking)
                query = query.AsNoTracking();

            return query.ToListAsync(ct);
        }

        public virtual async Task<List<Table>> ObterTodos(int page, int perPage, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            page = page <= 0 ? 1 : page;
            perPage = perPage <= 0 ? 10 : perPage;

            return await DbSet.AsNoTracking()
                              .Skip((page - 1) * perPage)
                              .Take(perPage)
                              .ToListAsync(ct);
        }
        public virtual async Task<IEnumerable<Table>> Pesquisar(Expression<Func<Table, bool>> predicate, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            return await DbSet.AsNoTracking().Where(predicate).ToListAsync(ct);
        }
        public virtual async Task<List<Table>> Pesquisar(Expression<Func<Table, bool>> predicate, CancellationToken ct = default,
                                                 params Expression<Func<Table, object>>[] includes)
        {
            ct.ThrowIfCancellationRequested();

            IQueryable<Table> q = DbSet.AsNoTracking();
            foreach (var inc in includes) q = q.Include(inc);
            return await q.Where(predicate).ToListAsync(ct);
        }
        #endregion

        #region New Methods

        public Task<int> AdicionarRange(IEnumerable<Table> itens, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var list = itens as ICollection<Table> ?? itens.ToList();
            DbSet.AddRange(list);
            return Task.FromResult(list.Count);
        }

        public Task<bool> Existe(Expression<Func<Table, bool>> predicate, CancellationToken ct = default)
                                            => DbSet.AsNoTracking().AnyAsync(predicate, ct);

        public Task<int> Contar(Expression<Func<Table, bool>>? predicate = null, CancellationToken ct = default)
                                             => (predicate is null ? DbSet : DbSet.Where(predicate)).CountAsync(ct);

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

        #region Dapper Queries

        protected async Task<List<T>> QueryListAsync<T>(string sql, object parameters, CancellationToken ct)
        {
            try
            {
                using SqlConnection connection = new(connectionString);

                var cmd = new CommandDefinition(
                    sql,
                    parameters: parameters,
                    commandTimeout: 900,
                    cancellationToken: ct
                );

                var resultado = await connection.QueryAsync<T>(cmd);
                return resultado?.AsList() ?? new List<T>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao executar QueryListAsync. SQL={Sql}, Params={@Params}", sql, parameters);
                return new List<T>();
            }
        }

        protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object parameters, CancellationToken ct)
        {
            try
            {
                using SqlConnection connection = new(connectionString);

                var cmd = new CommandDefinition(
                    sql,
                    parameters: parameters,
                    commandTimeout: 900,
                    cancellationToken: ct
                );

                return await connection.QueryFirstOrDefaultAsync<T>(cmd);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao executar QueryFirstOrDefaultAsync. SQL={Sql}, Params={@Params}", sql, parameters);
                return default;
            }
        }

        protected async Task<int> ExecuteAsync(string sql, object parameters, CancellationToken ct)
        {
            try
            {
                using SqlConnection connection = new(connectionString);

                var cmd = new CommandDefinition(
                    sql,
                    parameters: parameters,
                    commandTimeout: 900,
                    cancellationToken: ct
                );

                return await connection.ExecuteAsync(cmd);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao executar ExecuteAsync. SQL={Sql}, Params={@Params}", sql, parameters);
                return -1; // ou 0, se preferir representar "nenhuma linha afetada"
            }
        }

        protected async Task<T?> QueryFirstOrDefaultFromStoredProcedureAsync<T>(string procedureName, DynamicParameters parameters, CancellationToken ct)
        {
            try
            {
                using SqlConnection connection = new(connectionString);

                var cmd = new CommandDefinition(
                    procedureName,
                    parameters: parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct,
                    commandTimeout: 900
                );

                return await connection.QueryFirstOrDefaultAsync<T>(cmd);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao executar SP {ProcedureName}. Params={@Params}", procedureName, parameters);
                return default;
            }
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
