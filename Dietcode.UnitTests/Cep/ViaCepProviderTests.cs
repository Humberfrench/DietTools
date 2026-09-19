using System.Net;
using Dietcode.Core.Cep.Abstractions;
using Dietcode.Core.Cep.ViaCep;
using Dietcode.UnitTests.Cep.TestSupport;
using Xunit;

namespace Dietcode.UnitTests.Cep;

public class ViaCepProviderTests
{
    private const string ViaCepBaseUrl = "https://viacep.com.br/ws/";

    private static ViaCepProvider CreateProvider(StubHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ViaCepBaseUrl) };
        return new ViaCepProvider(httpClient);
    }

    [Fact]
    public async Task GetAddressAsync_ComCepValido_RetornaEndereco()
    {
        const string json = """
            {
              "cep": "01310-100",
              "logradouro": "Avenida Paulista",
              "complemento": "",
              "bairro": "Bela Vista",
              "localidade": "São Paulo",
              "uf": "SP",
              "ibge": "3550308",
              "gia": "1004",
              "ddd": "11",
              "siafi": "7107"
            }
            """;

        var provider = CreateProvider(StubHttpMessageHandler.ReturningJson(HttpStatusCode.OK, json));

        var result = await provider.GetAddressAsync("01310-100");

        Assert.True(result.IsSuccess);
        Assert.Equal("ViaCEP", result.Provider);
        Assert.False(result.Contingencia);
        Assert.Equal("Avenida Paulista", result.Address!.Logradouro);
        Assert.Equal("São Paulo", result.Address.Localidade);
        Assert.Equal("SP", result.Address.Uf);
    }

    [Fact]
    public async Task GetAddressAsync_ComErroBooleano_RetornaCepNaoEncontrado()
    {
        var provider = CreateProvider(StubHttpMessageHandler.ReturningJson(HttpStatusCode.OK, """{"erro": true}"""));

        var result = await provider.GetAddressAsync("00000000");

        Assert.False(result.IsSuccess);
        Assert.Equal("ViaCEP", result.Provider);
        Assert.Equal("CEP não encontrado.", result.Error);
    }

    [Fact]
    public async Task GetAddressAsync_ComErroComoString_RetornaCepNaoEncontrado()
    {
        // Particularidade real do ViaCEP: às vezes "erro" vem como string "true", não bool.
        var provider = CreateProvider(StubHttpMessageHandler.ReturningJson(HttpStatusCode.OK, """{"erro": "true"}"""));

        var result = await provider.GetAddressAsync("00000000");

        Assert.False(result.IsSuccess);
        Assert.Equal("CEP não encontrado.", result.Error);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("")]
    [InlineData("abcdefgh")]
    public async Task GetAddressAsync_ComCepEmFormatoInvalido_NaoFazChamadaHttp(string cepInvalido)
    {
        var provider = CreateProvider(StubHttpMessageHandler.ThrowingIfCalled());

        var result = await provider.GetAddressAsync(cepInvalido);

        Assert.False(result.IsSuccess);
        Assert.Equal("CEP inválido.", result.Error);
        Assert.Equal("ViaCEP", result.Provider);
    }

    [Fact]
    public async Task GetAddressAsync_ComFalhaDeServico_LancaCepProviderUnavailableException()
    {
        var provider = CreateProvider(StubHttpMessageHandler.ReturningStatus(HttpStatusCode.InternalServerError));

        var ex = await Assert.ThrowsAsync<CepProviderUnavailableException>(
            () => provider.GetAddressAsync("01310100"));

        Assert.Equal("ViaCEP", ex.Provider);
        Assert.Equal(500, ex.StatusCode);
    }
}
