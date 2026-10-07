namespace Dietcode.Core.Password.Models;

/// <summary>
/// Dados usados somente por regras locais (ex.: senha não pode conter o
/// username/e-mail). Nunca são enviados ao provider de senhas comprometidas.
/// </summary>
public sealed class PasswordValidationContext
{
    public string? UserName { get; init; }

    public string? Email { get; init; }

    public string? ApplicationName { get; init; }
}
