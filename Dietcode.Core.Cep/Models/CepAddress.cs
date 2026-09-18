namespace Dietcode.Core.Cep.Models;

public sealed class CepAddress
{
    public string Cep { get; init; } = string.Empty;

    public string Logradouro { get; init; } = string.Empty;

    public string Complemento { get; init; } = string.Empty;

    public string Bairro { get; init; } = string.Empty;

    public string Localidade { get; init; } = string.Empty;

    public string Uf { get; init; } = string.Empty;

    public string Ibge { get; init; } = string.Empty;

    public string Gia { get; init; } = string.Empty;

    public string Ddd { get; init; } = string.Empty;

    public string Siafi { get; init; } = string.Empty;
}
