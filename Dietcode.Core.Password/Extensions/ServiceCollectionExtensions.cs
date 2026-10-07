using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Hibp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dietcode.Core.Password.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra <see cref="IPasswordValidationService"/> e o provider de
    /// senha comprometida escolhido em <paramref name="primario"/> (hoje só
    /// HIBP). <paramref name="configureHibp"/> só é usado quando
    /// <paramref name="primario"/> é <see cref="PasswordProviderPrimario.Hibp"/>.
    /// </summary>
    public static IServiceCollection AddDietcodePassword(
        this IServiceCollection services,
        PasswordProviderPrimario primario = PasswordProviderPrimario.Hibp,
        Action<PasswordValidationOptions>? configureValidation = null,
        Action<HibpOptions>? configureHibp = null)
    {
        RegisterCore(services, configureValidation);
        RegisterProvider(services, primario, configureHibp);

        return services;
    }

    /// <summary>
    /// Mesma coisa, lendo <see cref="PasswordValidationOptions"/> de uma
    /// seção de <see cref="IConfiguration"/> (ex.: appsettings.json) — a
    /// subseção "Hibp" dela configura <see cref="HibpOptions"/>.
    /// </summary>
    public static IServiceCollection AddDietcodePassword(
        this IServiceCollection services,
        IConfiguration configuration,
        PasswordProviderPrimario primario = PasswordProviderPrimario.Hibp)
    {
        RegisterCore(services, configureValidation: null);
        services.Configure<PasswordValidationOptions>(configuration);

        RegisterProvider(services, primario, configureHibp: null);
        services.Configure<HibpOptions>(configuration.GetSection("Hibp"));

        return services;
    }

    private static void RegisterCore(IServiceCollection services, Action<PasswordValidationOptions>? configureValidation)
    {
        services.Configure(configureValidation ?? (_ => { }));
        services.AddMemoryCache();
        services.AddSingleton<IPasswordHashService, PasswordHashService>();
        services.AddScoped<IPasswordValidationService, PasswordValidationService>();
    }

    private static void RegisterProvider(
        IServiceCollection services,
        PasswordProviderPrimario primario,
        Action<HibpOptions>? configureHibp)
    {
        switch (primario)
        {
            case PasswordProviderPrimario.Hibp:
                services.Configure(configureHibp ?? (_ => { }));
                services.AddSingleton<IHibpRangeParser, HibpRangeParser>();

                services.AddHttpClient<HibpClient>((serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<HibpOptions>>().Value;
                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Dietcode.Core.Password/1.0");

                    if (options.UsePadding)
                        client.DefaultRequestHeaders.Add("Add-Padding", "true");
                });

                services.AddScoped<ICompromisedPasswordProvider, HibpCompromisedPasswordProvider>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(primario), primario, "Provider de senha não suportado.");
        }
    }
}
