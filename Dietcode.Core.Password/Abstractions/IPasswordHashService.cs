using Dietcode.Core.Password.Models;

namespace Dietcode.Core.Password.Abstractions;

public interface IPasswordHashService
{
    /// <summary>
    /// SHA-1 (UTF-8, hexadecimal maiúsculo) da senha exata — sem trim, sem
    /// ToUpper/ToLower. A senha é um valor exato; qualquer normalização
    /// silenciosa mudaria o hash e o resultado da consulta.
    /// </summary>
    PasswordHashRange CreateRange(string password);
}
