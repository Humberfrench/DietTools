using Dietcode.Core.DomainValidator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dietcode.Database.Domain
{
    public interface IBaseRepository<TEntity, Tipo> : IDisposable
        where TEntity : class, new()
    {
        Task<bool> Adicionar(TEntity obj);
        Task<bool> Atualizar(TEntity obj);
        Task<bool> Remover(TEntity obj);

        Task<TEntity> ObterPorId(Tipo id);
        Task<IEnumerable<TEntity>> ObterTodos();
        Task<IEnumerable<TEntity>> ObterTodosPaginado(int pagina, int registros);
        Task<IEnumerable<TEntity>> Pesquisar(Expression<Func<TEntity, bool>> predicate);

        void BeginTransaction();
        ValidationResult<TEntity> Commit();
    }
}
