using Dietcode.Core.Cep.Abstractions;
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
        if (configureOptions is not null)
            services.Configure(configureOptions);
        else
            services.AddOptions<ViaCepOptions>();

        services.AddHttpClient<ICepProvider, ViaCepProvider>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ViaCepOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
