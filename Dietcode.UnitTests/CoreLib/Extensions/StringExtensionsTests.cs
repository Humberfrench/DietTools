using Dietcode.Core.Lib;
using Xunit;

namespace Dietcode.UnitTests.CoreLib.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HasValue_ComTextoVazioOuNulo_RetornaFalse(string? valor)
    {
        Assert.False(valor.HasValue());
    }

    [Fact]
    public void HasValue_ComTexto_RetornaTrue()
    {
        Assert.True("Dietcode".HasValue());
    }

    [Fact]
    public void OnlyNumbers_RemoveCaracteresNaoNumericos()
    {
        Assert.Equal("11999999999", "(11) 99999-9999".OnlyNumbers());
    }

    [Fact]
    public void OnlyNumbers_ComTextoVazio_RetornaVazio()
    {
        Assert.Equal(string.Empty, "".OnlyNumbers());
    }

    [Fact]
    public void GetFirstName_RetornaPrimeiroNome()
    {
        Assert.Equal("Maria", "Maria Aparecida Silva".GetFirstName());
    }

    [Fact]
    public void GetFirstAndLastName_RetornaPrimeiroEUltimoNome()
    {
        Assert.Equal("Maria Silva", "Maria Aparecida Silva".GetFirstAndLastName());
    }

    [Theory]
    [InlineData("MinhaVariavel", "minha_variavel")]
    [InlineData("id", "id")]
    public void ToSnakeCase_ConverteCorretamente(string entrada, string esperado)
    {
        Assert.Equal(esperado, entrada.ToSnakeCase());
    }

    [Theory]
    [InlineData("minha_variavel", "minhaVariavel")]
    [InlineData("minha-variavel", "minhaVariavel")]
    public void ToCamelCase_ConverteCorretamente(string entrada, string esperado)
    {
        Assert.Equal(esperado, entrada.ToCamelCase());
    }

    [Fact]
    public void ToKebabCase_ConverteCorretamente()
    {
        Assert.Equal("minha-variavel", "MinhaVariavel".ToKebabCase());
    }

    [Fact]
    public void RemoveAccents_RemoveAcentuacao()
    {
        Assert.Equal("Acao", "Ação".RemoveAccents());
    }

    [Theory]
    [InlineData(true, "Sim")]
    [InlineData(false, "Não")]
    public void ToSimNao_Bool_ConverteCorretamente(bool valor, string esperado)
    {
        Assert.Equal(esperado, valor.ToSimNao());
    }

    [Theory]
    [InlineData("1123456789", "(11) 2345-6789")]
    [InlineData("11912345678", "(11) 91234-5678")]
    public void ToPhoneFormated_FormataTelefoneValido(string telefone, string esperado)
    {
        Assert.Equal(esperado, telefone.ToPhoneFormated());
    }

    [Fact]
    public void ToPhoneFormated_ComTamanhoInvalido_RetornaOriginal()
    {
        Assert.Equal("123", "123".ToPhoneFormated());
    }
}
