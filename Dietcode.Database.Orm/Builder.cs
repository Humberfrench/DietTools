using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Dietcode.Database.Orm.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Dietcode.Database.Orm
{
    public static class Builder
    {
        public static void BuilderStart(IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped(typeof(IMyContextManager<>), typeof(MyContextManager<>));
            services.AddScoped(typeof(IMyUnitOfWork<>), typeof(MyUnitOfWork<>));

            // novo: store agnóstico
            services.AddSingleton<IAmbientContextStore, AmbientContextStore>();
        }
    }
}
