using Dietcode.Core.Jobs.Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Dietcode.Core.Jobs.Redis;

/// <summary>
/// IJobQueue via Redis (lista + BRPOP). FIFO simples, um consumidor por vez — mesmo
/// contrato que uma fila em memória (ex.: Queue&lt;IJob&gt;) já cumpriria hoje.
/// </summary>
public sealed class RedisJobQueue : IJobQueue
{
    private readonly RedisJobConnection _connection;
    private readonly RedisJobOptions _options;

    internal RedisJobQueue(RedisJobConnection connection, IOptions<RedisJobOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }

    public async ValueTask EnqueueAsync(IJob job, CancellationToken ct)
    {
        var db = _connection.GetDatabase(_options.Database);

        // Hoje só existe um tipo de job em uso (GenericJob, de Dietcode.Core.Jobs) e ele
        // carrega apenas a IdempotencyKey — o worker busca HandlerKey/payload no store.
        // Por isso a fila guarda só a chave, sem envelope/tipo: é exatamente o que uma
        // fila em memória guardaria hoje (só o identificador do job).
        await db.ListLeftPushAsync(_options.QueueKey, job.IdempotencyKey);
    }

    public async ValueTask<IJob> DequeueAsync(CancellationToken ct)
    {
        var db = _connection.GetDatabase(_options.Database);

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            // BRPOP bloqueia no servidor Redis por até DequeuePollingSeconds; a lib não
            // cancela um comando em voo, então usamos um timeout curto e parametrizável e
            // checamos o CancellationToken a cada volta (latência de cancelamento limitada
            // a esse timeout).
            var result = await db.ExecuteAsync("BRPOP", _options.QueueKey, _options.DequeuePollingSeconds);

            if (result.IsNull)
                continue;

            var values = (RedisValue[])result!;
            var idempotencyKey = (string)values[1]!;

            return new GenericJob(idempotencyKey);
        }
    }
}
