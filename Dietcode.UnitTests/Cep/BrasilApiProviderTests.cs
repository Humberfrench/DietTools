using System.Net;
using Dietcode.Core.Cep.Abstractions;
using Dietcode.Core.Cep.BrasilApi;
using Dietcode.UnitTests.Cep.TestSupport;
using Xunit;

namespace Dietcode.UnitTests.Cep;

public class BrasilApiProviderTests
{
    private const string BrasilApiBaseUrl = "https://brasilapi.com.br/api/cep/v1/";

    private static BrasilApiProvider CreateProvider(StubHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BrasilApiBaseUrl) };
        return new BrasilApiProvider(httpClient);
    }

    [Fact]
    public async Task GetAddressAsync_ComCepValido_RetornaEndereco()
    {
        const string json = """
            {
              "cep": "89010025",
              "state": "SC",
              "city": "Blumenau",
              "neighborhood": "Centro",
              "street": "Rua Doutor Luiz de Freitas Melro",
              "service": "viacep",
              "ibge": { "city": "4202404", "state": "42" }
            }
            """;

        var provider = CreateProvider(StubHttpMessageHandler.ReturningJson(HttpStatusCode.OK, json));

        var result = await provider.GetAddressAsync("89010025");

        Assert.True(result.IsSuccess);
        Assert.Equal("BrasilAPI", result.Provider);
        Assert.False(result.Contingencia);
        Assert.Equal("Rua Doutor Luiz de Freitas Melro", result.Address!.Logradouro);
        Assert.Equal("Blumenau", result.Address.Localidade);
        Assert.Equal("SC", result.Address.Uf);
        Assert.Equal("4202404", result.Address.Ibge);
    }

    [Fact]
    public async Task GetAddressAsync_Com404_RetornaCepNaoEncontrado()
    {
        var provider = CreateProvider(StubHttpMessageHandler.ReturningStatus(HttpStatusCode.NotFound));

        var result = await provider.GetAddressAsync("00000000");

        Assert.False(result.IsSuccess);
        Assert.Equal("BrasilAPI", result.Provider);
        Assert.Equal("CEP não encontrado.", result.Error);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("")]
    public async Task GetAddressAsync_ComCepEmFormatoInvalido_NaoFazChamadaHttp(string cepInvalido)
    {
        var provider = CreateProvider(StubHttpMessageHandler.ThrowingIfCalled());

        var result = await provider.GetAddressAsync(cepInvalido);

        Assert.False(result.IsSuccess);
        Assert.Equal("CEP inválido.", result.Error);
        Assert.Equal("BrasilAPI", result.Provider);
    }

    [Fact]
    public async Task GetAddressAsync_ComFalhaDeServico_LancaCepProviderUnavailableException()
    {
        var provider = CreateProvider(StubHttpMessageHandler.ReturningStatus(HttpStatusCode.ServiceUnavailable));

        var ex = await Assert.ThrowsAsync<CepProviderUnavailableException>(
            () => provider.GetAddressAsync("89010025"));

        Assert.Equal("BrasilAPI", ex.Provider);
        Assert.Equal(503, ex.StatusCode);
    }
}
