using System.Text.Json.Serialization;

namespace Dietcode.Core.Cep.BrasilApi;

internal sealed class BrasilApiResponse
{
    [JsonPropertyName("cep")]
    public string Cep { get; init; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; init; } = string.Empty;

    [JsonPropertyName("neighborhood")]
    public string Neighborhood { get; init; } = string.Empty;

    [JsonPropertyName("street")]
    public string Street { get; init; } = string.Empty;

    [JsonPropertyName("service")]
    public string Service { get; init; } = string.Empty;

    [JsonPropertyName("ibge")]
    public BrasilApiIbge? Ibge { get; init; }
}

internal sealed class BrasilApiIbge
{
    [JsonPropertyName("city")]
    public string City { get; init; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;
}
