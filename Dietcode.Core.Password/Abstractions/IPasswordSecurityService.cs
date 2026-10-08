using Dietcode.Core.Password.Models;

namespace Dietcode.Core.Password.Abstractions;

/// <summary>
/// Combina <see cref="IPasswordValidationService"/> (regras locais + senha
/// vazada) e a análise de força por pontuação (<c>AnalyzePassword()</c>, em
/// <c>Dietcode.Core.Password.Scoring</c>) numa única chamada.
/// </summary>
public interface IPasswordSecurityService
{
    Task<PasswordSecurityResult> CheckAsync(
        string password,
        PasswordValidationContext? context = null,
        CancellationToken cancellationToken = default);
}
