namespace Dietcode.Core.Cep.Models;

public sealed class CepLookupResult
{
    public bool IsSuccess { get; init; }

    public CepAddress? Address { get; init; }

    public string Error { get; init; } = string.Empty;

    public static CepLookupResult Success(CepAddress address)
    {
        return new CepLookupResult
        {
            IsSuccess = true,
            Address = address
        };
    }

    public static CepLookupResult Failure(string error)
    {
        return new CepLookupResult
        {
            IsSuccess = false,
            Error = error
        };
    }
}
