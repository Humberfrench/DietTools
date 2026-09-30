using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Dietcode.Core.Jobs.Redis;

/// <summary>
/// Conexão Redis compartilhada entre <see cref="RedisJobQueue"/> e <see cref="RedisJobStore"/>,
/// para não abrir um ConnectionMultiplexer por classe. Detalhe interno de implementação:
/// a aplicação consumidora nunca referencia este tipo, só IJobQueue/IAsyncJobStoreGeneric.
/// </summary>
internal sealed class RedisJobConnection : IDisposable
{
    private readonly Lazy<ConnectionMultiplexer> _lazyMultiplexer;

    public RedisJobConnection(IOptions<RedisJobOptions> options)
    {
        var opts = options.Value;

        var errors = opts.Validate();
        if (errors.Count > 0)
            throw new ArgumentException($"RedisJobOptions inválido: {string.Join(" ", errors)}");

        _lazyMultiplexer = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(opts.ConnectionString));
    }

    public IDatabase GetDatabase(int? database) => _lazyMultiplexer.Value.GetDatabase(database ?? -1);

    public void Dispose()
    {
        if (_lazyMultiplexer.IsValueCreated)
            _lazyMultiplexer.Value.Dispose();
    }
}
