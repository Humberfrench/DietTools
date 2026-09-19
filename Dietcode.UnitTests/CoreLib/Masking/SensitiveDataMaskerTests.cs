using System.Text.Json;
using Dietcode.Core.Lib.Masking;
using Xunit;

namespace Dietcode.UnitTests.CoreLib.Masking;

public class SensitiveDataMaskerTests
{
    [Fact]
    public void Mask_EscondeCamposSensiveis()
    {
        var masked = SensitiveDataMasker.Mask(new
        {
            Email = "user@exemplo.com",
            Password = "123456",
            Token = "abc"
        });

        var json = JsonSerializer.Serialize(masked);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("user@exemplo.com", doc.RootElement.GetProperty("Email").GetString());
        Assert.Equal("***", doc.RootElement.GetProperty("Password").GetString());
        Assert.Equal("***", doc.RootElement.GetProperty("Token").GetString());
    }

    [Fact]
    public void Mask_ComObjetoAninhado_MascaraRecursivamente()
    {
        var masked = SensitiveDataMasker.Mask(new
        {
            Usuario = new { Nome = "Maria", Senha = "segredo" }
        });

        var json = JsonSerializer.Serialize(masked);
        using var doc = JsonDocument.Parse(json);

        var usuario = doc.RootElement.GetProperty("Usuario");
        Assert.Equal("Maria", usuario.GetProperty("Nome").GetString());
        Assert.Equal("***", usuario.GetProperty("Senha").GetString());
    }

    [Fact]
    public void Mask_ComValorNulo_RetornaNulo()
    {
        Assert.Null(SensitiveDataMasker.Mask(null));
    }
}
