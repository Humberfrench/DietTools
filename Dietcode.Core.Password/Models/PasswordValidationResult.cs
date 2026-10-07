namespace Dietcode.Core.Password.Models;

public sealed class PasswordValidationResult
{
    public bool IsValid { get; init; }

    public bool IsCompromised { get; init; }

    public long BreachOccurrences { get; init; }

    public PasswordCheckStatus BreachStatus { get; init; }

    public required IReadOnlyCollection<PasswordValidationIssue> Issues { get; init; }
}
