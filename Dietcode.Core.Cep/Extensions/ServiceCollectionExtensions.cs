using Dietcode.Core.Cep;
using Dietcode.Core.Cep.Abstractions;
using Dietcode.Core.Cep.BrasilApi;
using Dietcode.Core.Cep.ViaCep;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dietcode.Core.Cep.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDietcodeViaCep(
        this IServiceCollection services,
        Action<ViaCepOptions>? configureOptions = null)
    {
        ConfigureViaCep(services, configureOptions);

        services.AddHttpClient<ICepProvider, ViaCepProvider>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ViaCepOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }

    public static IServiceCollection AddDietcodeBrasilApi(
        this IServiceCollection services,
        Action<BrasilApiOptions>? configureOptions = null)
    {
        ConfigureBrasilApi(services, configureOptions);

        services.AddHttpClient<ICepProvider, BrasilApiProvider>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BrasilApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }

    /// <summary>Registra ViaCEP e BrasilAPI com failover automático: se o primário falhar, o secundário responde com <c>Contingencia = true</c>.</summary>
    public static IServiceCollection AddDietcodeCep(
        this IServiceCollection services,
        CepProviderPrimario primario = CepProviderPrimario.ViaCep,
        Action<ViaCepOptions>? configureViaCep = null,
        Action<BrasilApiOptions>? configureBrasilApi = null)
    {
        ConfigureViaCep(services, configureViaCep);
        ConfigureBrasilApi(services, configureBrasilApi);

        services.AddHttpClient<ViaCepProvider>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ViaCepOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddHttpClient<BrasilApiProvider>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BrasilApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddScoped<ICepProvider>(serviceProvider =>
        {
            var viaCep = serviceProvider.GetRequiredService<ViaCepProvider>();
            var brasilApi = serviceProvider.GetRequiredService<BrasilApiProvider>();

            return primario == CepProviderPrimario.ViaCep
                ? new ContingencyCepProvider(viaCep, brasilApi)
                : new ContingencyCepProvider(brasilApi, viaCep);
        });

        return services;
    }

    private static void ConfigureViaCep(IServiceCollection services, Action<ViaCepOptions>? configureOptions)
    {
        if (configureOptions is not null)
            services.Configure(configureOptions);
        else
            services.AddOptions<ViaCepOptions>();
    }

    private static void ConfigureBrasilApi(IServiceCollection services, Action<BrasilApiOptions>? configureOptions)
    {
        if (configureOptions is not null)
            services.Configure(configureOptions);
        else
            services.AddOptions<BrasilApiOptions>();
    }
}
