namespace Dietcode.Core.Email.Models;

public sealed class EmailMessage
{
    public EmailAddress? From { get; set; }

    public List<EmailAddress> To { get; set; } = [];

    public List<EmailAddress> Cc { get; set; } = [];

    public List<EmailAddress> Bcc { get; set; } = [];

    public List<EmailAddress> ReplyTo { get; set; } = [];

    public string Subject { get; set; } = string.Empty;

    public string? TextBody { get; set; }

    public string? HtmlBody { get; set; }

    public List<EmailAttachment> Attachments { get; set; } = [];

    public Dictionary<string, string> Headers { get; set; } = [];
}
