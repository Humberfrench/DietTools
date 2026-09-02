# Dietcode.Core.Email

Biblioteca para envio de emails via SMTP autenticado.

O pacote usa MailKit e foi separado do `Dietcode.Core.Lib` para evitar adicionar dependências de email em projetos que usam apenas utilitários gerais.

## Instalação

```bash
dotnet add package Dietcode.Core.Email --version 10.2.0
```

## Configuração

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

`Program.cs`, a partir de uma seção de configuração:

```csharp
using Dietcode.Core.Email.Extensions;

builder.Services.AddDietcodeSmtpEmail(
    builder.Configuration.GetSection("SmtpEmail"));
```

Ou configurando as opções diretamente em código, sem depender de `IConfiguration`:

```csharp
using Dietcode.Core.Email.Extensions;

builder.Services.AddDietcodeSmtpEmail(options =>
{
    options.Host = "smtp.seudominio.com.br";
    options.Port = 587;
    options.UseStartTls = true;
    options.UserName = "usuario@seudominio.com.br";
    options.Password = "senha-ou-app-password";
    options.FromEmail = "usuario@seudominio.com.br";
    options.FromName = "Dietcode";
});
```

Ambos os overloads de `AddDietcodeSmtpEmail` registram `IEmailSender` como `Transient`, implementado por `SmtpEmailSender`.

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
            TextBody = "Olá!",
            HtmlBody = "<strong>Olá!</strong>"
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            var error = result.Error;
        }
    }
}
```

`EmailAddress` também aceita conversão implícita a partir de `string`, então `To = ["cliente@exemplo.com"]` funciona diretamente.

## Mensagem

`EmailMessage` reúne todos os dados do envio:

- `From`: remetente; quando omitido, usa `FromEmail`/`FromName` da configuração.
- `To`, `Cc`, `Bcc`, `ReplyTo`: listas de `EmailAddress`.
- `Subject`, `TextBody`, `HtmlBody`.
- `Attachments`: lista de `EmailAttachment`.
- `Headers`: dicionário de cabeçalhos SMTP adicionais.

Validações antes do envio (falhas retornam `EmailSendResult.Failure`, sem lançar exceção): configuração SMTP válida (host, porta, remetente e timeout), pelo menos um destinatário (`To`, `Cc` ou `Bcc`), assunto obrigatório, corpo de texto/HTML ou ao menos um anexo, endereços não vazios e anexos com nome de arquivo e conteúdo.

## Anexos

```csharp
var message = new EmailMessage
{
    To = ["cliente@exemplo.com"],
    Subject = "Relatório",
    TextBody = "Segue o relatório em anexo.",
    Attachments =
    [
        EmailAttachment.FromBytes(
            fileName: "relatorio.pdf",
            content: pdfBytes,
            contentType: "application/pdf")
    ]
};
```

`EmailAttachment` também suporta anexos inline (`IsInline = true` com `ContentId`), úteis para imagens referenciadas dentro do `HtmlBody`.

## TLS/SSL

- Porta `587`: normalmente use `UseStartTls = true`.
- Porta `465`: normalmente use `UseSsl = true`.
- SMTP sem criptografia: use `UseSsl = false` e `UseStartTls = false`.

## Resultado

`SendAsync` retorna `EmailSendResult`:

- `IsSuccess`: indica se o envio foi concluído.
- `MessageId`: retorno do servidor SMTP quando disponível.
- `Error`: mensagem de erro quando o envio falha (validação ou exceção de transporte/autenticação).
- `TimeStamp`: data/hora UTC do resultado.

Cancelamentos por `CancellationToken` são propagados como `OperationCanceledException`; qualquer outra exceção durante o envio é convertida em `EmailSendResult.Failure`.

## Pacotes relacionados

- `Dietcode.Core.Email.Tester`: aplicação de console interna para testar manualmente o envio de email usando este pacote e um `appsettings.json` local.

## Licença

MIT
