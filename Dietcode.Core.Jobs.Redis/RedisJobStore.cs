using System.Text.Json;
using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Interfaces.Domain;
using Microsoft.Extensions.Options;

namespace Dietcode.Core.Jobs.Redis;

/// <summary>
/// IAsyncJobStoreGeneric via Redis: cada job vira uma chave String (JSON), mesmo papel
/// que um Dictionary&lt;string, AsyncJobStateGeneric&gt; em memória cumpriria hoje. TTL
/// opcional via RedisJobOptions.StateTimeToLive (nulo = sem expiração, igual ao dicionário).
/// </summary>
public sealed class RedisJobStore : IAsyncJobStoreGeneric
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private readonly RedisJobConnection _connection;
    private readonly RedisJobOptions _options;

    internal RedisJobStore(RedisJobConnection connection, IOptions<RedisJobOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }

    public async Task CreateAsync(AsyncJobStateGeneric job, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var db = _connection.GetDatabase(_options.Database);
        var json = JsonSerializer.Serialize(job, JsonOpts);

        await db.StringSetAsync(Key(job.IdempotencyKey), json, _options.StateTimeToLive);
    }

    public async Task<AsyncJobStateGeneric?> GetAsync(string idempotencyKey, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var db = _connection.GetDatabase(_options.Database);
        var value = await db.StringGetAsync(Key(idempotencyKey));

        return value.IsNullOrEmpty
            ? null
            : JsonSerializer.Deserialize<AsyncJobStateGeneric>(value!, JsonOpts);
    }

    public Task SetCompletedAsync(string idempotencyKey, string resultJson, CancellationToken ct)
        => UpdateAsync(idempotencyKey, state =>
        {
            state.Status = JobStatus.Completed;
            state.ResultJson = resultJson;
            state.CompletedAtUtc = DateTime.UtcNow;
        }, ct);

    public Task SetFailedAsync(string idempotencyKey, string error, CancellationToken ct)
        => UpdateAsync(idempotencyKey, state =>
        {
            state.Status = JobStatus.Failed;
            state.Error = error;
            state.CompletedAtUtc = DateTime.UtcNow;
        }, ct);

    private async Task UpdateAsync(string idempotencyKey, Action<AsyncJobStateGeneric> mutate, CancellationToken ct)
    {
        // Read-modify-write: sem lock/transação Redis. Suficiente para o mesmo cenário de
        // uso de hoje (um worker sequencial por job); não é uma garantia forte de
        // concorrência multi-writer no mesmo idempotencyKey.
        var state = await GetAsync(idempotencyKey, ct);
        if (state is null)
            return;

        mutate(state);

        var db = _connection.GetDatabase(_options.Database);
        var json = JsonSerializer.Serialize(state, JsonOpts);

        await db.StringSetAsync(Key(idempotencyKey), json, _options.StateTimeToLive, when: StackExchange.Redis.When.Exists);
    }

    private string Key(string idempotencyKey) => $"{_options.StoreKeyPrefix}{idempotencyKey}";
}
