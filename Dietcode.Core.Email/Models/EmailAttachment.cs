namespace Dietcode.Core.Email.Models;

public sealed class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/octet-stream";

    public byte[] Content { get; set; } = [];

    public bool IsInline { get; set; }

    public string? ContentId { get; set; }

    public static EmailAttachment FromBytes(
        string fileName,
        byte[] content,
        string contentType = "application/octet-stream")
    {
        return new EmailAttachment
        {
            FileName = fileName,
            Content = content,
            ContentType = contentType
        };
    }
}
