using System.Linq.Expressions;
using Dietcode.Core.DomainValidator;

namespace Dietcode.Database.Domain
{
    public interface IBaseRepository<TEntity, Tipo> : IDisposable
        where TEntity : class, new()
    {

        Task<bool> Adicionar(TEntity obj, CancellationToken ct = default);
        Task<bool> Atualizar(TEntity obj, CancellationToken ct = default);
        Task<bool> Remover(TEntity obj, CancellationToken ct = default);

        Task<TEntity> ObterPorId(Tipo id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> ObterTodos(CancellationToken ct = default);
        Task<IEnumerable<TEntity>> ObterTodos(int pagina, int registros, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> Pesquisar(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

        void BeginTransaction();
        Task<ValidationResult<TEntity>> Commit(CancellationToken ct = default);

    }
}
