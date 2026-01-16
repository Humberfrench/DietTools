using Dietcode.Database.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Dietcode.Database.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDietcodeDatabase(
            this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(DapperRepository<>));
            services.AddScoped<DapperUnitOfWork>();

            return services;
        }
    }
}
