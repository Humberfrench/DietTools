using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;


namespace Dietcode.Database.Orm
{
    public class BaseRepository<T1, T2, T3, T4, T5, T6> : Repository<T1>, IBaseRepository<T1> where T1 : class, new() where T2 : class, new()
                                                                          where T3 : class, new() where T4 : class, new()
                                                                          where T5 : class, new() where T6 : class, new()
    {

        public BaseRepository(IMyContextManager<ThisDatabase<T1>> context,
                              IMyContextManager<ThisDatabase<T1, T2, T3, T4, T5, T6>> contextManager) : base(context)
        {
        }

        #region Dispose
        public new void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
