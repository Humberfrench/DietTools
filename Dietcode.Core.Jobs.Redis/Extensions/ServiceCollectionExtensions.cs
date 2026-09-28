using Dietcode.Core.Jobs.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dietcode.Core.Jobs.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDietcodeRedisJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RedisJobOptions>(configuration);
        return AddRedisJobs(services);
    }

    public static IServiceCollection AddDietcodeRedisJobs(
        this IServiceCollection services,
        Action<RedisJobOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return AddRedisJobs(services);
    }

    private static IServiceCollection AddRedisJobs(IServiceCollection services)
    {
        services.AddSingleton<RedisJobConnection>();

        services.AddSingleton<IJobQueue>(sp =>
            new RedisJobQueue(
                sp.GetRequiredService<RedisJobConnection>(),
                sp.GetRequiredService<IOptions<RedisJobOptions>>()));

        services.AddSingleton<IAsyncJobStoreGeneric>(sp =>
            new RedisJobStore(
                sp.GetRequiredService<RedisJobConnection>(),
                sp.GetRequiredService<IOptions<RedisJobOptions>>()));

        return services;
    }
}
