using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database
{
    public interface IRepository<T>
    {
        Task<T> Get(int id);
        Task<IEnumerable<T>> Get();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(int id);

    }
}
