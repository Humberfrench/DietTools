namespace Dietcode.Core.Cep.BrasilApi;

public sealed class BrasilApiOptions
{
    public string BaseUrl { get; set; } = "https://brasilapi.com.br/api/cep/v1/";

    public int TimeoutSeconds { get; set; } = 30;
}
