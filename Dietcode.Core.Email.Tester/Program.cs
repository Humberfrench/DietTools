using Dietcode.Core.Email.Abstractions;
using Dietcode.Core.Email.Extensions;
using Dietcode.Core.Email.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var services = new ServiceCollection();

services.AddDietcodeSmtpEmail(configuration.GetSection("SmtpEmail"));

using var provider = services.BuildServiceProvider();

var emailSender = provider.GetRequiredService<IEmailSender>();
var to = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
    ? args[0]
    : "hga@dietcode.com.br";
var now = DateTimeOffset.Now;

var result = await emailSender.SendAsync(new EmailMessage
{
    To = [new EmailAddress(to)],
    Subject = $"Teste SMTP Dietcode.Core.Email - {now:yyyy-MM-dd HH:mm:ss}",
    TextBody = $"Email de teste enviado por Dietcode.Core.Email em {now:yyyy-MM-dd HH:mm:ss zzz}.",
    HtmlBody = $"""
        <html>
            <body>
                <h2>Teste SMTP Dietcode.Core.Email</h2>
                <p>Email de teste enviado em <strong>{now:yyyy-MM-dd HH:mm:ss zzz}</strong>.</p>
            </body>
        </html>
        """
});

if (result.IsSuccess)
{
    Console.WriteLine("Email enviado com sucesso.");
    Console.WriteLine($"MessageId: {result.MessageId}");
    return;
}

Console.WriteLine("Falha ao enviar email.");
Console.WriteLine(result.Error);
Environment.ExitCode = 1;
