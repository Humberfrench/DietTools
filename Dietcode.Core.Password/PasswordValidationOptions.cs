namespace Dietcode.Core.Password;

public sealed class PasswordValidationOptions
{
    public int MinimumLength { get; set; } = 12;

    public int MaximumLength { get; set; } = 128;

    public bool CheckCompromisedPasswords { get; set; } = true;

    public bool RejectCompromisedPasswords { get; set; } = true;

    /// <summary>O que fazer quando o provider de senha comprometida está indisponível.</summary>
    public ProviderFailurePolicy ProviderFailurePolicy { get; set; } = ProviderFailurePolicy.FailClosed;
}
