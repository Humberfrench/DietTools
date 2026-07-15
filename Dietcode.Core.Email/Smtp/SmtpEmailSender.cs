using Dietcode.Core.Email.Abstractions;
using Dietcode.Core.Email.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Dietcode.Core.Email.Smtp;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpEmailOptions _options;

    public SmtpEmailSender(IOptions<SmtpEmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task<EmailSendResult> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(message);
        if (validationErrors.Count > 0)
            return EmailSendResult.Failure(string.Join(" ", validationErrors));

        try
        {
            var mimeMessage = CreateMessage(message);

            using var smtp = new SmtpClient
            {
                Timeout = _options.TimeoutSeconds * 1000
            };

            await smtp.ConnectAsync(
                _options.Host,
                _options.Port,
                ResolveSecureSocketOptions(),
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await smtp.AuthenticateAsync(
                    _options.UserName,
                    _options.Password,
                    cancellationToken);
            }

            var messageId = await smtp.SendAsync(mimeMessage, cancellationToken);

            await smtp.DisconnectAsync(true, cancellationToken);

            return EmailSendResult.Success(messageId);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return EmailSendResult.Failure(ex.Message);
        }
    }

    private MimeMessage CreateMessage(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();

        var from = message.From ?? new EmailAddress(_options.FromEmail, _options.FromName);
        mimeMessage.From.Add(ToMailboxAddress(from));

        AddAddresses(mimeMessage.To, message.To);
        AddAddresses(mimeMessage.Cc, message.Cc);
        AddAddresses(mimeMessage.Bcc, message.Bcc);
        AddAddresses(mimeMessage.ReplyTo, message.ReplyTo);

        mimeMessage.Subject = message.Subject;

        foreach (var header in message.Headers)
            mimeMessage.Headers[header.Key] = header.Value;

        var body = new BodyBuilder
        {
            TextBody = message.TextBody,
            HtmlBody = message.HtmlBody
        };

        foreach (var attachment in message.Attachments)
        {
            var contentType = ContentType.Parse(attachment.ContentType);

            if (attachment.IsInline)
            {
                var linked = body.LinkedResources.Add(
                    attachment.FileName,
                    attachment.Content,
                    contentType);

                if (!string.IsNullOrWhiteSpace(attachment.ContentId))
                    linked.ContentId = attachment.ContentId;

                continue;
            }

            body.Attachments.Add(
                attachment.FileName,
                attachment.Content,
                contentType);
        }

        mimeMessage.Body = body.ToMessageBody();

        return mimeMessage;
    }

    private List<string> Validate(EmailMessage message)
    {
        var errors = new List<string>(_options.Validate());

        if (!message.To.Any() && !message.Cc.Any() && !message.Bcc.Any())
            errors.Add("Informe ao menos um destinatario.");

        if (string.IsNullOrWhiteSpace(message.Subject))
            errors.Add("Assunto obrigatorio.");

        if (string.IsNullOrWhiteSpace(message.TextBody) &&
            string.IsNullOrWhiteSpace(message.HtmlBody) &&
            message.Attachments.Count == 0)
        {
            errors.Add("Informe corpo do email ou ao menos um anexo.");
        }

        foreach (var address in message.To.Concat(message.Cc).Concat(message.Bcc).Concat(message.ReplyTo))
        {
            if (string.IsNullOrWhiteSpace(address.Email))
                errors.Add("Endereco de email vazio informado.");
        }

        foreach (var attachment in message.Attachments)
        {
            if (string.IsNullOrWhiteSpace(attachment.FileName))
                errors.Add("Anexo sem nome de arquivo.");

            if (attachment.Content.Length == 0)
                errors.Add($"Anexo '{attachment.FileName}' sem conteudo.");
        }

        return errors;
    }

    private SecureSocketOptions ResolveSecureSocketOptions()
    {
        if (_options.UseSsl)
            return SecureSocketOptions.SslOnConnect;

        if (_options.UseStartTls)
            return SecureSocketOptions.StartTls;

        return SecureSocketOptions.None;
    }

    private static void AddAddresses(InternetAddressList target, IEnumerable<EmailAddress> addresses)
    {
        foreach (var address in addresses)
            target.Add(ToMailboxAddress(address));
    }

    private static MailboxAddress ToMailboxAddress(EmailAddress address)
        => new(address.Name ?? string.Empty, address.Email);
}
