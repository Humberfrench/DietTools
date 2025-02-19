using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database
{
    public interface IRepository<T>
    {
        Task<T> GetAsync(int id);

        Task<IEnumerable<T>> GetAsync();

        Task<int> AddAsync(T entity);

        Task<bool> UpdateAsync(T entity);

        Task<bool> DeleteAsync(int id);

        T Get(int id);

        IEnumerable<T> Get();

        long Add(T entity);

        bool Update(T entity);

        bool Delete(int id);

    }
}
