namespace Dietcode.Core.Password.Models;

public sealed record PasswordValidationIssue(string Code, string Message);

/// <summary>Códigos de <see cref="PasswordValidationIssue"/> produzidos por <c>PasswordValidationService</c>.</summary>
public static class PasswordValidationIssueCodes
{
    public const string PasswordRequired = "PASSWORD_REQUIRED";
    public const string PasswordTooShort = "PASSWORD_TOO_SHORT";
    public const string PasswordTooLong = "PASSWORD_TOO_LONG";
    public const string PasswordCompromised = "PASSWORD_COMPROMISED";
    public const string PasswordContainsUsername = "PASSWORD_CONTAINS_USERNAME";
    public const string PasswordContainsEmail = "PASSWORD_CONTAINS_EMAIL";
    public const string PasswordProviderUnavailable = "PASSWORD_PROVIDER_UNAVAILABLE";
}
