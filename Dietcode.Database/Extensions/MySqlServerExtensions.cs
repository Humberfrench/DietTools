using Dietcode.Database.Abstractions;
using Dietcode.Database.DatabaseProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Dietcode.Database.Extensions
{
    public static class MySqlServerExtensions
    {
        public static IServiceCollection AddDietcodeMySql(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddScoped<IConnectionFactory>(
                _ => new MySqlConnectionFactory(connectionString));

            services.AddDietcodeDatabase();

            return services;
        }
    }
}

