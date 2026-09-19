using Dietcode.Core.Cep.Models;
using Xunit;

namespace Dietcode.UnitTests.Cep;

public class CepLookupResultTests
{
    [Fact]
    public void Success_PreencheEnderecoEProvider()
    {
        var address = new CepAddress { Cep = "01310100", Logradouro = "Avenida Paulista" };

        var result = CepLookupResult.Success(address, "ViaCEP");

        Assert.True(result.IsSuccess);
        Assert.Same(address, result.Address);
        Assert.Equal("ViaCEP", result.Provider);
        Assert.False(result.Contingencia);
        Assert.Equal(string.Empty, result.Error);
    }

    [Fact]
    public void Failure_PreencheErroEProvider()
    {
        var result = CepLookupResult.Failure("CEP não encontrado.", "BrasilAPI");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Address);
        Assert.Equal("CEP não encontrado.", result.Error);
        Assert.Equal("BrasilAPI", result.Provider);
    }

    [Fact]
    public void With_AlteraApenasContingencia()
    {
        var original = CepLookupResult.Success(new CepAddress(), "ViaCEP");

        var comContingencia = original with { Contingencia = true };

        Assert.False(original.Contingencia);
        Assert.True(comContingencia.Contingencia);
        Assert.Equal(original.Provider, comContingencia.Provider);
        Assert.Equal(original.IsSuccess, comContingencia.IsSuccess);
        Assert.Same(original.Address, comContingencia.Address);
    }
}
