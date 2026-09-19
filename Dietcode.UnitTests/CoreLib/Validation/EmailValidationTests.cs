using Dietcode.Core.Lib;
using Xunit;

namespace Dietcode.UnitTests.CoreLib.Validation;

public class EmailValidationTests
{
    [Theory]
    [InlineData("usuario@exemplo.com")]
    [InlineData("usuario.nome+tag@exemplo.com.br")]
    public void IsValidEmail_ComEmailValido_RetornaTrue(string email)
    {
        Assert.True(email.IsValidEmail());
    }

    [Theory]
    [InlineData("")]
    [InlineData("nao-e-email")]
    [InlineData("@exemplo.com")]
    [InlineData("usuario@")]
    public void IsValidEmail_ComEmailInvalido_RetornaFalse(string email)
    {
        Assert.False(email.IsValidEmail());
    }
}
