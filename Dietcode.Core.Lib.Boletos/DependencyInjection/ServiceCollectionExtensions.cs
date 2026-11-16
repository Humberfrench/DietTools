using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dietcode.Core.Lib.Boletos.DependencyInjection;

/// <summary>Registro da engine de boletos na injeção de dependência.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra <see cref="IBoletoValidator"/> como singleton, usando
    /// <see cref="TimeProvider.System"/> a menos que outro <see cref="TimeProvider"/> já esteja registrado.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="configure">Ajusta as políticas padrão (<see cref="BoletoValidacaoOptions"/>).</param>
    public static IServiceCollection AddDietcodeBoletos(
        this IServiceCollection services,
        Action<BoletoValidacaoOptions>? configure = null)
    {
        services.Configure(configure ?? (_ => { }));
        RegisterCore(services);

        return services;
    }

    /// <summary>Mesma coisa, lendo <see cref="BoletoValidacaoOptions"/> de uma seção de configuração.</summary>
    public static IServiceCollection AddDietcodeBoletos(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<BoletoValidacaoOptions>(configuration);
        RegisterCore(services);

        return services;
    }

    private static void RegisterCore(IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IBoletoValidator, BoletoValidator>();
    }
}
