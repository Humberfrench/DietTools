using Dietcode.Database.Abstractions;
using Dietcode.Database.DatabaseProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Dietcode.Database.Extensions
{
    public static class PostgreSqlExtensions
    {
        public static IServiceCollection AddDietcodePostgreSql(
         this IServiceCollection services,
         string connectionString)
        {
            services.AddScoped<IConnectionFactory>(
                _ => new PostgreSqlConnectionFactory(connectionString));

            services.AddDietcodeDatabase();

            return services;
        }
    }
}
