using Dietcode.Core.Email.Abstractions;
using Dietcode.Core.Email.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dietcode.Core.Email.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDietcodeSmtpEmail(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpEmailOptions>(configuration);
        services.AddTransient<IEmailSender, SmtpEmailSender>();

        return services;
    }

    public static IServiceCollection AddDietcodeSmtpEmail(
        this IServiceCollection services,
        Action<SmtpEmailOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddTransient<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
