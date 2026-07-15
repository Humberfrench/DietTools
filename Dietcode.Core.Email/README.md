# Dietcode.Core.Email

Biblioteca para envio de emails via SMTP autenticado.

O pacote usa MailKit e foi separado do `Dietcode.Core.Lib` para evitar adicionar dependencias de email em projetos que usam apenas utilitarios gerais.

## Instalacao

```bash
dotnet add package Dietcode.Core.Email --version 1.0.0
```

## Configuracao

`appsettings.json`:

```json
{
  "SmtpEmail": {
    "Host": "smtp.seudominio.com.br",
    "Port": 587,
    "UseSsl": false,
    "UseStartTls": true,
    "UserName": "usuario@seudominio.com.br",
    "Password": "senha-ou-app-password",
    "FromEmail": "usuario@seudominio.com.br",
    "FromName": "Dietcode",
    "TimeoutSeconds": 120
  }
}
```

`Program.cs`:

```csharp
using Dietcode.Core.Email.Extensions;

builder.Services.AddDietcodeSmtpEmail(
    builder.Configuration.GetSection("SmtpEmail"));
```

## Uso

```csharp
using Dietcode.Core.Email.Abstractions;
using Dietcode.Core.Email.Models;

public sealed class WelcomeService
{
    private readonly IEmailSender _emailSender;

    public WelcomeService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendAsync(CancellationToken cancellationToken)
    {
        var result = await _emailSender.SendAsync(new EmailMessage
        {
            To = [new EmailAddress("cliente@exemplo.com", "Cliente")],
            Subject = "Bem-vindo",
            TextBody = "Ola!",
            HtmlBody = "<strong>Ola!</strong>"
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            var error = result.Error;
        }
    }
}
```

## Anexos

```csharp
var message = new EmailMessage
{
    To = ["cliente@exemplo.com"],
    Subject = "Relatorio",
    TextBody = "Segue o relatorio em anexo.",
    Attachments =
    [
        EmailAttachment.FromBytes(
            fileName: "relatorio.pdf",
            content: pdfBytes,
            contentType: "application/pdf")
    ]
};
```

## TLS/SSL

- Porta `587`: normalmente use `UseStartTls = true`.
- Porta `465`: normalmente use `UseSsl = true`.
- SMTP sem criptografia: use `UseSsl = false` e `UseStartTls = false`.

## Resultado

`SendAsync` retorna `EmailSendResult`:

- `IsSuccess`: indica se o envio foi concluido.
- `MessageId`: retorno do servidor SMTP quando disponivel.
- `Error`: mensagem de erro quando o envio falha.
- `TimeStamp`: data/hora UTC do resultado.

Cancelamentos por `CancellationToken` sao propagados como `OperationCanceledException`.

## Licenca

MIT
