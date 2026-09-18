namespace Dietcode.Core.Cep.Models;

public sealed record CepLookupResult
{
    public bool IsSuccess { get; init; }

    public CepAddress? Address { get; init; }

    public string Error { get; init; } = string.Empty;

    public string Provider { get; init; } = string.Empty;

    public bool Contingencia { get; init; }

    public static CepLookupResult Success(CepAddress address, string provider, bool contingencia = false)
    {
        return new CepLookupResult
        {
            IsSuccess = true,
            Address = address,
            Provider = provider,
            Contingencia = contingencia
        };
    }

    public static CepLookupResult Failure(string error, string provider, bool contingencia = false)
    {
        return new CepLookupResult
        {
            IsSuccess = false,
            Error = error,
            Provider = provider,
            Contingencia = contingencia
        };
    }
}
