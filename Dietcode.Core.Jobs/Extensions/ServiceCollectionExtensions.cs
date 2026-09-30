using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Memory;
using Dietcode.Core.Jobs.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dietcode.Core.Jobs.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra tudo que Dietcode.Core.Jobs precisa: JobAsyncService, GenericJobHandler,
    /// JobWorkerGeneric e o par IJobQueue/IAsyncJobStoreGeneric do provider escolhido em
    /// <paramref name="primario"/>. <paramref name="configureRedis"/> só é usado quando
    /// <paramref name="primario"/> é Redis (ignorado em InMemory). A aplicação ainda
    /// precisa registrar seu próprio IHandlerDispatcher.
    /// </summary>
    public static IServiceCollection AddDietcodeJobs(
        this IServiceCollection services,
        JobsProviderPrimario primario,
        Action<RedisJobOptions>? configureRedis = null)
    {
        RegisterCore(services);
        RegisterProvider(services, primario, configureRedis);

        return services;
    }

    /// <summary>
    /// Mesma coisa, mas lendo RedisJobOptions de uma seção de IConfiguration (ex.:
    /// appsettings.json) em vez de uma lambda. Só é relevante quando primario é Redis.
    /// </summary>
    public static IServiceCollection AddDietcodeJobs(
        this IServiceCollection services,
        JobsProviderPrimario primario,
        IConfiguration redisConfiguration)
    {
        RegisterCore(services);
        services.Configure<RedisJobOptions>(redisConfiguration);
        RegisterProvider(services, primario, configureRedis: null);

        return services;
    }

    private static void RegisterCore(IServiceCollection services)
    {
        services.AddScoped(typeof(IJobAsyncService<,>), typeof(JobAsyncService<,>));
        services.AddScoped<IJobHandler<GenericJob>, GenericJobHandler>();
        services.AddHostedService<JobWorkerGeneric>();
    }

    private static void RegisterProvider(
        IServiceCollection services,
        JobsProviderPrimario primario,
        Action<RedisJobOptions>? configureRedis)
    {
        switch (primario)
        {
            case JobsProviderPrimario.Redis:
                // Garante IOptions<RedisJobOptions> resolvível mesmo sem host que já
                // tenha chamado AddOptions() (ex.: console app puro).
                services.Configure(configureRedis ?? (_ => { }));

                services.AddSingleton<RedisJobConnection>();

                services.AddSingleton<IJobQueue>(sp =>
                    new RedisJobQueue(
                        sp.GetRequiredService<RedisJobConnection>(),
                        sp.GetRequiredService<IOptions<RedisJobOptions>>()));

                services.AddSingleton<IAsyncJobStoreGeneric>(sp =>
                    new RedisJobStore(
                        sp.GetRequiredService<RedisJobConnection>(),
                        sp.GetRequiredService<IOptions<RedisJobOptions>>()));
                break;

            case JobsProviderPrimario.InMemory:
                services.AddSingleton<IJobQueue, InMemoryJobQueue>();
                services.AddSingleton<IAsyncJobStoreGeneric, InMemoryJobStore>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(primario), primario, "Provider de Jobs nao suportado.");
        }
    }
}
