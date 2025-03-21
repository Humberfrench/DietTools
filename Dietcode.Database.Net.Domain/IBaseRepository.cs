using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dietcode.Database.Net.Domain
{
    public interface IBaseRepository<TEntity> : IDisposable
    {
        Task<bool> Adicionar(TEntity obj);

        Task<bool> Atualizar(TEntity obj);

        Task<bool> Remover(TEntity obj);

        Task<TEntity> ObterPorId(int id);

        Task<IEnumerable<TEntity>> ObterTodos();

        Task<IEnumerable<TEntity>> ObterTodosPaginado(int pagina, int registros);

        Task<IEnumerable<TEntity>> Pesquisar(Expression<Func<TEntity, bool>> predicate);
    }
}
