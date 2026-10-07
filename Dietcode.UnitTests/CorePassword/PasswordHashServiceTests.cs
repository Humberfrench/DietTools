using System.Security.Cryptography;
using System.Text;
using Dietcode.Core.Password;
using Xunit;

namespace Dietcode.UnitTests.CorePassword;

public class PasswordHashServiceTests
{
    private readonly PasswordHashService _service = new();

    [Fact]
    public void CreateRange_ReproduzShaConhecido_ComPrefixoESufixoCorretos()
    {
        var range = _service.CreateRange("password");

        var expectedHash = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes("password")));

        Assert.Equal(expectedHash[..5], range.Prefix);
        Assert.Equal(expectedHash[5..], range.Suffix);
        Assert.Equal(40, range.Prefix.Length + range.Suffix.Length);
    }

    [Fact]
    public void CreateRange_RetornaHexadecimalMaiusculo()
    {
        var range = _service.CreateRange("qualquer-senha-123");

        Assert.Matches("^[0-9A-F]{5}$", range.Prefix);
        Assert.Matches("^[0-9A-F]{35}$", range.Suffix);
    }

    [Fact]
    public void CreateRange_EhCaseSensitive()
    {
        var lower = _service.CreateRange("senha123");
        var upper = _service.CreateRange("SENHA123");

        Assert.NotEqual(lower.Prefix + lower.Suffix, upper.Prefix + upper.Suffix);
    }

    [Fact]
    public void CreateRange_NaoNormalizaEspacos_ValorExatoImportaParaOHash()
    {
        var comEspacos = _service.CreateRange(" Senha Çafé ");
        var semEspacos = _service.CreateRange("Senha Çafé");

        Assert.NotEqual(comEspacos.Prefix + comEspacos.Suffix, semEspacos.Prefix + semEspacos.Suffix);
    }

    [Fact]
    public void CreateRange_SuportaUnicodeViaUtf8()
    {
        // Não deve lançar exceção, e deve produzir hash diferente de uma senha puramente ASCII.
        var comAcentos = _service.CreateRange("Senhaçãoéü123");
        var semAcentos = _service.CreateRange("Senhacaoeu123");

        Assert.NotEqual(comAcentos.Prefix + comAcentos.Suffix, semAcentos.Prefix + semAcentos.Suffix);
    }
}
