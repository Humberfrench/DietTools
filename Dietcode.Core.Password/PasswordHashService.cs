using System.Security.Cryptography;
using System.Text;
using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Models;

namespace Dietcode.Core.Password;

/// <summary>
/// SHA-1 é usado aqui somente porque faz parte do protocolo de consulta do
/// Pwned Passwords (k-anonymity). Nunca deve ser usado como algoritmo de
/// armazenamento de senha.
/// </summary>
public sealed class PasswordHashService : IPasswordHashService
{
    private const int PrefixLength = 5;

    public PasswordHashRange CreateRange(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = Convert.ToHexString(SHA1.HashData(bytes));

        return new PasswordHashRange(
            Prefix: hash[..PrefixLength],
            Suffix: hash[PrefixLength..]);
    }
}
