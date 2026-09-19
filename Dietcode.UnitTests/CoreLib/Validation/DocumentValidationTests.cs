using Dietcode.Core.Lib;
using Xunit;

namespace Dietcode.UnitTests.CoreLib.Validation;

public class DocumentValidationTests
{
    [Theory]
    [InlineData("111.444.777-35")]
    [InlineData("11144477735")]
    public void IsCpf_ComCpfValido_RetornaTrue(string cpf)
    {
        Assert.True(Validacao.IsCpf(cpf));
    }

    [Theory]
    [InlineData("111.444.777-30")] // dígito verificador errado
    [InlineData("11111111111")] // todos os dígitos iguais
    [InlineData("123")] // tamanho inválido
    [InlineData("")]
    public void IsCpf_ComCpfInvalido_RetornaFalse(string cpf)
    {
        Assert.False(Validacao.IsCpf(cpf));
    }

    [Theory]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11222333000181")]
    public void IsCnpj_ComCnpjValido_RetornaTrue(string cnpj)
    {
        Assert.True(Validacao.IsCnpj(cnpj));
    }

    [Theory]
    [InlineData("11.222.333/0001-80")] // dígito verificador errado
    [InlineData("11111111111111")] // todos os dígitos iguais
    [InlineData("123")] // tamanho inválido
    public void IsCnpj_ComCnpjInvalido_RetornaFalse(string cnpj)
    {
        Assert.False(Validacao.IsCnpj(cnpj));
    }

    [Fact]
    public void ToCpf_ComOnzeDigitos_Formata()
    {
        Assert.Equal("111.444.777-35", "11144477735".ToCpf());
    }

    [Fact]
    public void ToCpf_ComTamanhoDiferente_RetornaOriginal()
    {
        Assert.Equal("123", "123".ToCpf());
    }

    [Fact]
    public void ToCnpj_ComCatorzeDigitos_Formata()
    {
        Assert.Equal("11.222.333/0001-81", "11222333000181".ToCnpj());
    }

    [Theory]
    [InlineData("11144477735", "111.444.777-35")]
    [InlineData("11222333000181", "11.222.333/0001-81")]
    public void FormatoCpfouCnpj_DetectaAutomaticamente(string documento, string esperado)
    {
        Assert.Equal(esperado, documento.FormatoCpfouCnpj());
    }
}
