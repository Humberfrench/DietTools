using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> Get(int id);
        Task<IEnumerable<T>> Get();
        Task<int> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(int id);

    }
}
