namespace Dietcode.Core.Password.Models;

/// <summary>
/// Prefixo (enviado ao provider) e sufixo (permanece local) do hash SHA-1 de
/// uma senha, para consulta por k-anonymity.
/// </summary>
public sealed record PasswordHashRange(string Prefix, string Suffix);
