using Dietcode.Core.Email.Models;

namespace Dietcode.Core.Email.Abstractions;

public interface IEmailSender
{
    Task<EmailSendResult> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);
}
