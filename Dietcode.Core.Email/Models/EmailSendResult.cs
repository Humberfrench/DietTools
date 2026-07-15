namespace Dietcode.Core.Email.Models;

public sealed class EmailSendResult
{
    public bool IsSuccess { get; init; }

    public string MessageId { get; init; } = string.Empty;

    public string Error { get; init; } = string.Empty;

    public DateTimeOffset TimeStamp { get; init; } = DateTimeOffset.UtcNow;

    public static EmailSendResult Success(string messageId)
    {
        return new EmailSendResult
        {
            IsSuccess = true,
            MessageId = messageId
        };
    }

    public static EmailSendResult Failure(string error)
    {
        return new EmailSendResult
        {
            IsSuccess = false,
            Error = error
        };
    }
}
