namespace Dietcode.Core.Password.Models;

public sealed class PasswordBreachResult
{
    public bool IsCompromised { get; init; }

    public long Occurrences { get; init; }

    public required PasswordCheckStatus Status { get; init; }

    public string? Provider { get; init; }

    public static PasswordBreachResult Safe(string provider)
        => new() { IsCompromised = false, Occurrences = 0, Status = PasswordCheckStatus.Safe, Provider = provider };

    public static PasswordBreachResult Compromised(long occurrences, string provider)
        => new() { IsCompromised = true, Occurrences = occurrences, Status = PasswordCheckStatus.Compromised, Provider = provider };

    public static PasswordBreachResult Unavailable(string provider)
        => new() { IsCompromised = false, Occurrences = 0, Status = PasswordCheckStatus.ProviderUnavailable, Provider = provider };

    public static PasswordBreachResult InvalidInput(string provider)
        => new() { IsCompromised = false, Occurrences = 0, Status = PasswordCheckStatus.InvalidInput, Provider = provider };
}
