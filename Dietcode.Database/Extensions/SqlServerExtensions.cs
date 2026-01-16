using Dietcode.Database.Abstractions;
using Dietcode.Database.DatabaseProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Dietcode.Database.Extensions
{
    public static class SqlServerExtensions
    {
        public static IServiceCollection AddDietcodeSqlServer(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddScoped<IConnectionFactory>(
                _ => new SqlServerConnectionFactory(connectionString));

            services.AddDietcodeDatabase();

            return services;
        }
    }
}

