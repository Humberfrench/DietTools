using Dietcode.Core.Email.Models;
using Dietcode.Core.Email.Smtp;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dietcode.UnitTests.Email;

// Cobre apenas o caminho de validação de SmtpEmailSender.SendAsync, que roda
// ANTES de qualquer conexão SMTP real. Envio bem-sucedido exigiria um
// servidor SMTP (real ou fake) e não é coberto aqui.
public class SmtpEmailSenderValidationTests
{
    private static SmtpEmailSender CreateSender(SmtpEmailOptions? options = null)
    {
        return new SmtpEmailSender(Options.Create(options ?? ValidOptions()));
    }

    private static SmtpEmailOptions ValidOptions() => new()
    {
        Host = "smtp.exemplo.com.br",
        Port = 587,
        FromEmail = "contato@exemplo.com.br",
        TimeoutSeconds = 30
    };

    [Fact]
    public async Task SendAsync_SemHostConfigurado_RetornaFalha()
    {
        var sender = CreateSender(new SmtpEmailOptions { FromEmail = "a@b.com" });

        var result = await sender.SendAsync(new EmailMessage
        {
            To = ["cliente@exemplo.com"],
            Subject = "Assunto",
            TextBody = "Corpo"
        });

        Assert.False(result.IsSuccess);
        Assert.Contains("SMTP Host obrigatorio.", result.Error);
    }

    [Fact]
    public async Task SendAsync_SemDestinatario_RetornaFalha()
    {
        var sender = CreateSender();

        var result = await sender.SendAsync(new EmailMessage
        {
            Subject = "Assunto",
            TextBody = "Corpo"
        });

        Assert.False(result.IsSuccess);
        Assert.Contains("Informe ao menos um destinatario.", result.Error);
    }

    [Fact]
    public async Task SendAsync_SemAssunto_RetornaFalha()
    {
        var sender = CreateSender();

        var result = await sender.SendAsync(new EmailMessage
        {
            To = ["cliente@exemplo.com"],
            TextBody = "Corpo"
        });

        Assert.False(result.IsSuccess);
        Assert.Contains("Assunto obrigatorio.", result.Error);
    }

    [Fact]
    public async Task SendAsync_SemCorpoNemAnexo_RetornaFalha()
    {
        var sender = CreateSender();

        var result = await sender.SendAsync(new EmailMessage
        {
            To = ["cliente@exemplo.com"],
            Subject = "Assunto"
        });

        Assert.False(result.IsSuccess);
        Assert.Contains("Informe corpo do email ou ao menos um anexo.", result.Error);
    }

    [Fact]
    public async Task SendAsync_ComAnexoSemNomeDeArquivo_RetornaFalha()
    {
        var sender = CreateSender();

        var result = await sender.SendAsync(new EmailMessage
        {
            To = ["cliente@exemplo.com"],
            Subject = "Assunto",
            Attachments =
            [
                EmailAttachment.FromBytes(fileName: "", content: [1, 2, 3])
            ]
        });

        Assert.False(result.IsSuccess);
        Assert.Contains("Anexo sem nome de arquivo.", result.Error);
    }

    [Fact]
    public async Task SendAsync_ComMultiplosErrosDeValidacao_AgregaTodasAsMensagens()
    {
        var sender = CreateSender(new SmtpEmailOptions());

        var result = await sender.SendAsync(new EmailMessage());

        Assert.False(result.IsSuccess);
        Assert.Contains("SMTP Host obrigatorio.", result.Error);
        Assert.Contains("Informe ao menos um destinatario.", result.Error);
        Assert.Contains("Assunto obrigatorio.", result.Error);
    }
}
