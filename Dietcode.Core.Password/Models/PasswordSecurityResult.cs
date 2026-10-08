using Dietcode.Core.Password.Scoring;

namespace Dietcode.Core.Password.Models;

/// <summary>
/// Resultado combinado de <see cref="IPasswordSecurityService.CheckAsync"/>:
/// a validação (regras locais + senha vazada) e a força por pontuação,
/// lado a lado. <see cref="IsValid"/> reflete só a validação — a força é
/// informativa, não bloqueante (ver README, seção "Scoring de força de
/// senha").
/// </summary>
public sealed class PasswordSecurityResult
{
    public required PasswordValidationResult Validation { get; init; }

    public required PasswordStrengthResult Strength { get; init; }

    public bool IsValid => Validation.IsValid;
}
