using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

////MUST Add
//{
//    services.AddScoped(typeof(IMyUnitOfWork<>), typeof(MyUnitOfWork<>));
//    services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
//    services.AddScoped(typeof(IMyContextManager<>), typeof(MyContextManager<>));

//}

namespace Dietcode.Database.Orm
{
    public class BaseRepository<T1>(IMyContextManager<ThisDatabase<T1>> contextManager) : Repository<T1>(contextManager), IBaseRepository<T1> where T1 : class, new()
    {

        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
