using System.Linq.Expressions;
using Dietcode.Core.DomainValidator;

namespace Dietcode.Database.Domain
{
    public interface IBaseRepository<Table, Tipo> : IDisposable
        where Table : class, new()
    {

        Task<bool> Adicionar(Table obj, CancellationToken ct = default);
        Task<bool> Atualizar(Table obj, CancellationToken ct = default);
        Task<bool> Remover(Table obj, CancellationToken ct = default);

        Task<Table?> ObterPorId(Tipo id, bool asTracking = false, CancellationToken ct = default);
        Task<List<Table>> ObterTodos(bool asTracking = false, CancellationToken ct = default);
        Task<List<Table>> ObterTodos(int pagina, int registros, CancellationToken ct = default);
        Task<IEnumerable<Table>> Pesquisar(Expression<Func<Table, bool>> predicate, CancellationToken ct = default);
        Task<List<Table>> Pesquisar(Expression<Func<Table, bool>> predicate, CancellationToken ct = default,
                                                 params Expression<Func<Table, object>>[] includes);
        //news
        public Task<bool> Existe(Expression<Func<Table, bool>> predicate, CancellationToken ct = default);
        public Task<int> Contar(Expression<Func<Table, bool>>? predicate = null, CancellationToken ct = default);
        Task<int> AdicionarRange(IEnumerable<Table> itens, CancellationToken ct = default);

        void BeginTransaction();
        Task<ValidationResult<Table>> Commit(CancellationToken ct = default);

    }
}
