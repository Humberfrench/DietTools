using System.Net;
using System.Text.Json;
using Dietcode.Core.Cep.Abstractions;
using Dietcode.Core.Cep.Models;

namespace Dietcode.Core.Cep.BrasilApi;

public sealed class BrasilApiProvider : ICepProvider
{
    internal const string ProviderName = "BrasilAPI";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public BrasilApiProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CepLookupResult> GetAddressAsync(string cep, CancellationToken cancellationToken = default)
    {
        var digits = CepNormalizer.Normalize(cep);
        if (digits is null)
            return CepLookupResult.Failure("CEP inválido.", ProviderName);

        using var httpResponse = await _httpClient.GetAsync(digits, cancellationToken);

        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
            return CepLookupResult.Failure("CEP não encontrado.", ProviderName);

        if (!httpResponse.IsSuccessStatusCode)
            throw new CepProviderUnavailableException(ProviderName, (int)httpResponse.StatusCode);

        var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<BrasilApiResponse>(stream, _jsonOptions, cancellationToken);
        if (payload is null)
            return CepLookupResult.Failure("CEP não encontrado.", ProviderName);

        return CepLookupResult.Success(new CepAddress
        {
            Cep = payload.Cep,
            Logradouro = payload.Street,
            Bairro = payload.Neighborhood,
            Localidade = payload.City,
            Uf = payload.State,
            Ibge = payload.Ibge?.City ?? string.Empty
        }, ProviderName);
    }
}
