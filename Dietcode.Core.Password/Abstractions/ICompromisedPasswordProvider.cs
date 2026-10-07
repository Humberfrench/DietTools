using Dietcode.Core.Password.Models;

namespace Dietcode.Core.Password.Abstractions;

/// <summary>
/// Verifica se uma senha já apareceu em bases conhecidas de senhas
/// comprometidas. Implementações nunca enviam a senha, o hash completo ou o
/// sufixo do hash a terceiros — só o prefixo (k-anonymity).
/// </summary>
public interface ICompromisedPasswordProvider
{
    Task<PasswordBreachResult> CheckAsync(string password, CancellationToken cancellationToken = default);
}
