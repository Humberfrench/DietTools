namespace Dietcode.Core.Cep.ViaCep;

public sealed class ViaCepOptions
{
    public string BaseUrl { get; set; } = "https://viacep.com.br/ws/";

    public int TimeoutSeconds { get; set; } = 30;
}
