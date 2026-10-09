using System.Net;
using System.Text;
using Dietcode.Core.Password;
using Dietcode.Core.Password.Hibp;
using Dietcode.Core.Password.Models;
using Dietcode.UnitTests.Cep.TestSupport;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dietcode.UnitTests.CorePassword.Hibp;

public class HibpCompromisedPasswordProviderTests
{
    private static HibpCompromisedPasswordProvider CreateProvider(
        StubHttpMessageHandler handler,
        HibpOptions? options = null)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.pwnedpasswords.com/") };
        var hibpClient = new HibpClient(httpClient);
        var hashService = new PasswordHashService();
        var parser = new HibpRangeParser();
        var cache = new MemoryCache(new MemoryCacheOptions());

        return new HibpCompromisedPasswordProvider(hibpClient, hashService, parser, cache, Options.Create(options ?? new HibpOptions()));
    }

    private static StubHttpMessageHandler ReturningText(HttpStatusCode statusCode, string body)
        => new(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, "text/plain")
        });

    //[Fact]
    //public async Task CheckAsync_SenhaComprometida_RetornaCompromised()
    //{
    //    var hashService = new PasswordHashService();
    //    var range = hashService.CreateRange("password");

    //    var handler = ReturningText(HttpStatusCode.OK, $"{range.Suffix}:37");
    //    var provider = CreateProvider(handler);

    //    var result = await provider.CheckAsync("password");

    //    Assert.True(result.IsCompromised);
    //    Assert.Equal(37, result.Occurrences);
    //    Assert.Equal(PasswordCheckStatus.Compromised, result.Status);
    //    Assert.Equal(HibpCompromisedPasswordProvider.ProviderName, result.Provider);
    //}

    [Fact]
    public async Task CheckAsync_SenhaNaoEncontrada_RetornaSafe()
    {
        var handler = ReturningText(HttpStatusCode.OK, "0000000000000000000000000000000000000:5");
        var provider = CreateProvider(handler);

        var result = await provider.CheckAsync("uma-senha-bem-aleatoria-qualquer-2024");

        Assert.False(result.IsCompromised);
        Assert.Equal(PasswordCheckStatus.Safe, result.Status);
    }

    [Fact]
    public async Task CheckAsync_Http500_RetornaProviderUnavailable()
    {
        var handler = StubHttpMessageHandler.ReturningStatus(HttpStatusCode.InternalServerError);
        var provider = CreateProvider(handler);

        var result = await provider.CheckAsync("qualquer-senha");

        Assert.Equal(PasswordCheckStatus.ProviderUnavailable, result.Status);
        Assert.False(result.IsCompromised);
    }

    [Fact]
    public async Task CheckAsync_Http403_RetornaProviderUnavailable()
    {
        var handler = StubHttpMessageHandler.ReturningStatus(HttpStatusCode.Forbidden);
        var provider = CreateProvider(handler);

        var result = await provider.CheckAsync("qualquer-senha");

        Assert.Equal(PasswordCheckStatus.ProviderUnavailable, result.Status);
    }

    [Fact]
    public async Task CheckAsync_Cancelamento_LancaOperationCanceled()
    {
        var provider = CreateProvider(StubHttpMessageHandler.ThrowingIfCalled());

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => provider.CheckAsync("qualquer-senha", cts.Token));
    }

    [Fact]
    public async Task CheckAsync_SenhaVazia_RetornaInvalidInputSemChamarHttp()
    {
        var provider = CreateProvider(StubHttpMessageHandler.ThrowingIfCalled());

        var result = await provider.CheckAsync("");

        Assert.Equal(PasswordCheckStatus.InvalidInput, result.Status);
    }

    [Fact]
    public async Task CheckAsync_ProviderDesabilitado_NaoFazChamadaHttpERetornaSafe()
    {
        var provider = CreateProvider(StubHttpMessageHandler.ThrowingIfCalled(), new HibpOptions { Enabled = false });

        var result = await provider.CheckAsync("qualquer-senha");

        Assert.Equal(PasswordCheckStatus.Safe, result.Status);
    }
}
