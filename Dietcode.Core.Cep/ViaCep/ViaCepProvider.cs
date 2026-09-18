using System.Net;
using System.Text.Json;
using Dietcode.Core.Cep.Abstractions;
using Dietcode.Core.Cep.Models;

namespace Dietcode.Core.Cep.ViaCep;

public sealed class ViaCepProvider : ICepProvider
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public ViaCepProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CepLookupResult> GetAddressAsync(string cep, CancellationToken cancellationToken = default)
    {
        var digits = NormalizeCep(cep);
        if (digits is null)
            return CepLookupResult.Failure("CEP inválido.");

        using var httpResponse = await _httpClient.GetAsync($"{digits}/json/", cancellationToken);

        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
            return CepLookupResult.Failure("CEP não encontrado.");

        if (!httpResponse.IsSuccessStatusCode)
            return CepLookupResult.Failure($"Falha ao consultar o ViaCEP (HTTP {(int)httpResponse.StatusCode}).");

        var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<ViaCepResponse>(stream, _jsonOptions, cancellationToken);
        if (payload is null || payload.Erro)
            return CepLookupResult.Failure("CEP não encontrado.");

        return CepLookupResult.Success(new CepAddress
        {
            Cep = payload.Cep,
            Logradouro = payload.Logradouro,
            Complemento = payload.Complemento,
            Bairro = payload.Bairro,
            Localidade = payload.Localidade,
            Uf = payload.Uf,
            Ibge = payload.Ibge,
            Gia = payload.Gia,
            Ddd = payload.Ddd,
            Siafi = payload.Siafi
        });
    }

    private static string? NormalizeCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return null;

        var digits = new string(cep.Where(char.IsDigit).ToArray());

        return digits.Length == 8 ? digits : null;
    }
}
