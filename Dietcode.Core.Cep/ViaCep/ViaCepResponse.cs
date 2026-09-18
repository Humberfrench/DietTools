using System.Text.Json.Serialization;

namespace Dietcode.Core.Cep.ViaCep;

internal sealed class ViaCepResponse
{
    [JsonPropertyName("cep")]
    public string Cep { get; init; } = string.Empty;

    [JsonPropertyName("logradouro")]
    public string Logradouro { get; init; } = string.Empty;

    [JsonPropertyName("complemento")]
    public string Complemento { get; init; } = string.Empty;

    [JsonPropertyName("bairro")]
    public string Bairro { get; init; } = string.Empty;

    [JsonPropertyName("localidade")]
    public string Localidade { get; init; } = string.Empty;

    [JsonPropertyName("uf")]
    public string Uf { get; init; } = string.Empty;

    [JsonPropertyName("ibge")]
    public string Ibge { get; init; } = string.Empty;

    [JsonPropertyName("gia")]
    public string Gia { get; init; } = string.Empty;

    [JsonPropertyName("ddd")]
    public string Ddd { get; init; } = string.Empty;

    [JsonPropertyName("siafi")]
    public string Siafi { get; init; } = string.Empty;

    [JsonPropertyName("erro")]
    public bool Erro { get; init; }
}
