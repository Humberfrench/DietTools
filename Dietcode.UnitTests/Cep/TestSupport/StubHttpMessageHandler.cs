namespace Dietcode.UnitTests.Cep.TestSupport;

/// <summary>Handler HTTP falso, sem rede, para testar providers de CEP com respostas controladas.</summary>
internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    public static StubHttpMessageHandler ReturningJson(System.Net.HttpStatusCode statusCode, string json)
    {
        return new StubHttpMessageHandler(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });
    }

    public static StubHttpMessageHandler ReturningStatus(System.Net.HttpStatusCode statusCode)
    {
        return new StubHttpMessageHandler(_ => new HttpResponseMessage(statusCode));
    }

    public static StubHttpMessageHandler ThrowingIfCalled()
    {
        return new StubHttpMessageHandler(_ => throw new InvalidOperationException(
            "O provider não deveria ter feito nenhuma chamada HTTP para este cenário."));
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_responder(request));
    }
}
