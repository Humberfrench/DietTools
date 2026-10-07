using System.Net;
using System.Text;
using Dietcode.Core.Password;
using Dietcode.Core.Password.Hibp;
using Dietcode.UnitTests.Cep.TestSupport;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dietcode.UnitTests.CorePassword;

/// <summary>
/// Teste de privacidade obrigatório (spec, seção 29): garante que a
/// requisição HTTP ao provider de senha comprometida carrega só o prefixo de
/// 5 caracteres do hash SHA-1 — nunca a senha, o hash completo, o sufixo,
/// username ou e-mail. Protege uma propriedade arquitetural de segurança, não
/// só um detalhe de implementação.
/// </summary>
public class PasswordPrivacyTests
{
    [Fact]
    public async Task CheckAsync_RequisicaoHttp_ContemApenasOPrefixoDoHash()
    {
        const string password = "MinhaSenhaSecretaQueNuncaDeveSerEnviada!2024";
        const string userName = "usuario.secreto";
        const string email = "usuario.secreto@exemplo.com";

        var hashService = new PasswordHashService();
        var range = hashService.CreateRange(password);

        HttpRequestMessage? capturedRequest = null;

        var handler = new StubHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("AAAA1111AAAA1111AAAA1111AAAA1111AAAA1:1", Encoding.UTF8, "text/plain")
            };
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.pwnedpasswords.com/") };
        var provider = new HibpCompromisedPasswordProvider(
            new HibpClient(httpClient),
            hashService,
            new HibpRangeParser(),
            new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new HibpOptions()));

        // userName/email só existem aqui pra provar que não aparecem na requisição — o
        // provider nem sequer os recebe (ICompromisedPasswordProvider.CheckAsync só toma a senha).
        await provider.CheckAsync(password);

        Assert.NotNull(capturedRequest);

        var fullUrl = capturedRequest!.RequestUri!.ToString();
        Assert.Equal($"https://api.pwnedpasswords.com/range/{range.Prefix}", fullUrl);

        var headerText = string.Join(" ", capturedRequest.Headers.Select(h => $"{h.Key}:{string.Join(",", h.Value)}"));
        var requestText = $"{fullUrl} {headerText}";

        Assert.DoesNotContain(password, requestText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(range.Suffix, requestText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(range.Prefix + range.Suffix, requestText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(userName, requestText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(email, requestText, StringComparison.OrdinalIgnoreCase);
    }
}
