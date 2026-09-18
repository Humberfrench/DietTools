using Dietcode.Core.Cep.Models;

namespace Dietcode.Core.Cep.Abstractions;

public interface ICepProvider
{
    Task<CepLookupResult> GetAddressAsync(string cep, CancellationToken cancellationToken = default);
}
