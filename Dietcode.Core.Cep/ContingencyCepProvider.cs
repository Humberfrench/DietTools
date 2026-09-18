using Dietcode.Core.Cep.Abstractions;
using Dietcode.Core.Cep.Models;

namespace Dietcode.Core.Cep;

public sealed class ContingencyCepProvider : ICepProvider
{
    private readonly ICepProvider _primario;
    private readonly ICepProvider _secundario;

    public ContingencyCepProvider(ICepProvider primario, ICepProvider secundario)
    {
        _primario = primario;
        _secundario = secundario;
    }

    public async Task<CepLookupResult> GetAddressAsync(string cep, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _primario.GetAddressAsync(cep, cancellationToken);
        }
        catch (Exception ex) when (IsFalhaDeServico(ex, cancellationToken))
        {
            var resultado = await _secundario.GetAddressAsync(cep, cancellationToken);

            return resultado with { Contingencia = true };
        }
    }

    private static bool IsFalhaDeServico(Exception ex, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return false;

        return ex is CepProviderUnavailableException or HttpRequestException or TaskCanceledException;
    }
}
