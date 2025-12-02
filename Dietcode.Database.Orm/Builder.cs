using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Dietcode.Database.Orm.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database.Orm
{
    public static class Builder
    {
        public static void BuilderStart(IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped(typeof(IMyContextManager<>), typeof(MyContextManager<>));
            services.AddScoped(typeof(IMyUnitOfWork<>), typeof(MyUnitOfWork<>));

        }
    }
}
