using Dietcode.Core.Lib.Passwords;
using Xunit;

namespace Dietcode.UnitTests.CoreLib.Passwords;

public class PasswordStrengthTests
{
    [Fact]
    public void AnalyzePassword_ComSenhaForte_AtendeRegrasMinimas()
    {
        var result = "Senha@123".AsSpan().AnalyzePassword();

        Assert.True(result.HasUppercase);
        Assert.True(result.HasLowercase);
        Assert.True(result.HasDigit);
        Assert.True(result.HasSymbol);
        Assert.True(result.MeetsMinimumRules);
        Assert.NotEqual(PasswordStrengthLevel.Invalid, result.Level);
    }

    [Fact]
    public void AnalyzePassword_ComSenhaCurtaESemComplexidade_NaoAtendeRegrasMinimas()
    {
        var result = "abc".AsSpan().AnalyzePassword();

        Assert.False(result.MeetsMinimumRules);
        Assert.Equal(PasswordStrengthLevel.VeryWeak, result.Level);
    }

    [Fact]
    public void AnalyzePassword_ComEspacoEmBranco_EhInvalida()
    {
        var result = "abc 123!A".AsSpan().AnalyzePassword();

        Assert.Equal(PasswordStrengthLevel.Invalid, result.Level);
        Assert.False(result.MeetsMinimumRules);
    }

    [Fact]
    public void AnalyzePassword_ComCaractereNaoAscii_EhInvalida()
    {
        var result = "Senhaçã123!".AsSpan().AnalyzePassword();

        Assert.Equal(PasswordStrengthLevel.Invalid, result.Level);
    }

    [Fact]
    public void AnalyzePassword_ComTextoVazio_RetornaResultadoVazio()
    {
        var result = "".AsSpan().AnalyzePassword();

        Assert.Equal(0, result.Length);
        Assert.False(result.MeetsMinimumRules);
    }
}
