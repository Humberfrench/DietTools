using Dietcode.Core.Cep;
using Dietcode.Core.Cep.Abstractions;
using Dietcode.Core.Cep.Models;
using Xunit;

namespace Dietcode.UnitTests.Cep;

public class ContingencyCepProviderTests
{
    private sealed class FakeCepProvider : ICepProvider
    {
        private readonly Func<string, CancellationToken, Task<CepLookupResult>> _handler;

        public FakeCepProvider(Func<string, CancellationToken, Task<CepLookupResult>> handler)
        {
            _handler = handler;
        }

        public int CallCount { get; private set; }

        public Task<CepLookupResult> GetAddressAsync(string cep, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return _handler(cep, cancellationToken);
        }
    }

    private static CepAddress SampleAddress() => new() { Cep = "01310100", Logradouro = "Avenida Paulista" };

    [Fact]
    public async Task GetAddressAsync_QuandoPrimarioTemSucesso_NaoChamaOSecundario()
    {
        var primario = new FakeCepProvider((_, _) =>
            Task.FromResult(CepLookupResult.Success(SampleAddress(), "ViaCEP")));
        var secundario = new FakeCepProvider((_, _) =>
            throw new InvalidOperationException("Não deveria ter sido chamado."));

        var contingency = new ContingencyCepProvider(primario, secundario);

        var result = await contingency.GetAddressAsync("01310100");

        Assert.True(result.IsSuccess);
        Assert.Equal("ViaCEP", result.Provider);
        Assert.False(result.Contingencia);
        Assert.Equal(1, primario.CallCount);
        Assert.Equal(0, secundario.CallCount);
    }

    [Fact]
    public async Task GetAddressAsync_QuandoPrimarioFalhaPorServicoIndisponivel_ChamaOSecundarioEMarcaContingencia()
    {
        var primario = new FakeCepProvider((_, _) =>
            throw new CepProviderUnavailableException("ViaCEP", 500));
        var secundario = new FakeCepProvider((_, _) =>
            Task.FromResult(CepLookupResult.Success(SampleAddress(), "BrasilAPI")));

        var contingency = new ContingencyCepProvider(primario, secundario);

        var result = await contingency.GetAddressAsync("01310100");

        Assert.True(result.IsSuccess);
        Assert.Equal("BrasilAPI", result.Provider);
        Assert.True(result.Contingencia);
        Assert.Equal(1, primario.CallCount);
        Assert.Equal(1, secundario.CallCount);
    }

    [Fact]
    public async Task GetAddressAsync_QuandoPrimarioFalhaComHttpRequestException_ChamaOSecundario()
    {
        var primario = new FakeCepProvider((_, _) =>
            throw new HttpRequestException("Falha de rede."));
        var secundario = new FakeCepProvider((_, _) =>
            Task.FromResult(CepLookupResult.Success(SampleAddress(), "BrasilAPI")));

        var contingency = new ContingencyCepProvider(primario, secundario);

        var result = await contingency.GetAddressAsync("01310100");

        Assert.True(result.Contingencia);
        Assert.Equal(1, secundario.CallCount);
    }

    [Fact]
    public async Task GetAddressAsync_QuandoPrimarioRetornaCepNaoEncontrado_NaoAcionaContingencia()
    {
        var primario = new FakeCepProvider((_, _) =>
            Task.FromResult(CepLookupResult.Failure("CEP não encontrado.", "ViaCEP")));
        var secundario = new FakeCepProvider((_, _) =>
            throw new InvalidOperationException("CEP não encontrado não deveria acionar o secundário."));

        var contingency = new ContingencyCepProvider(primario, secundario);

        var result = await contingency.GetAddressAsync("00000000");

        Assert.False(result.IsSuccess);
        Assert.Equal("ViaCEP", result.Provider);
        Assert.False(result.Contingencia);
        Assert.Equal(0, secundario.CallCount);
    }

    [Fact]
    public async Task GetAddressAsync_ComCancelamentoDoChamador_PropagaExcecaoSemAcionarSecundario()
    {
        using var cts = new CancellationTokenSource();

        var primario = new FakeCepProvider((_, ct) =>
        {
            cts.Cancel();
            ct.ThrowIfCancellationRequested();
            return Task.FromResult(CepLookupResult.Success(SampleAddress(), "ViaCEP"));
        });
        var secundario = new FakeCepProvider((_, _) =>
            throw new InvalidOperationException("Não deveria ter sido chamado."));

        var contingency = new ContingencyCepProvider(primario, secundario);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => contingency.GetAddressAsync("01310100", cts.Token));

        Assert.Equal(0, secundario.CallCount);
    }
}
