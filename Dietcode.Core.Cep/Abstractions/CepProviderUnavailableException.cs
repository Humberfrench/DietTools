namespace Dietcode.Core.Cep.Abstractions;

public sealed class CepProviderUnavailableException : Exception
{
    public string Provider { get; }

    public int? StatusCode { get; }

    public CepProviderUnavailableException(string provider, int? statusCode = null, Exception? innerException = null)
        : base(BuildMessage(provider, statusCode), innerException)
    {
        Provider = provider;
        StatusCode = statusCode;
    }

    private static string BuildMessage(string provider, int? statusCode)
    {
        return statusCode.HasValue
            ? $"Provedor de CEP '{provider}' indisponível (HTTP {statusCode.Value})."
            : $"Provedor de CEP '{provider}' indisponível.";
    }
}
