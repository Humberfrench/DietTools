using System.Collections.Concurrent;
using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Interfaces.Domain;

namespace Dietcode.Core.Jobs.Memory;

/// <summary>
/// IAsyncJobStoreGeneric em memória via ConcurrentDictionary: não sobrevive a restart
/// nem é compartilhado entre instâncias da aplicação. É o provider padrão — igual a
/// qualquer outro (ex.: Redis), implementa o mesmo contrato IAsyncJobStoreGeneric.
/// </summary>
public sealed class InMemoryJobStore : IAsyncJobStoreGeneric
{
    private readonly ConcurrentDictionary<string, AsyncJobStateGeneric> _jobs = new();

    public Task CreateAsync(AsyncJobStateGeneric job, CancellationToken ct)
    {
        _jobs[job.IdempotencyKey] = job;
        return Task.CompletedTask;
    }

    public Task<AsyncJobStateGeneric?> GetAsync(string idempotencyKey, CancellationToken ct)
        => Task.FromResult(_jobs.TryGetValue(idempotencyKey, out var job) ? job : null);

    public Task SetCompletedAsync(string idempotencyKey, string resultJson, CancellationToken ct)
    {
        if (_jobs.TryGetValue(idempotencyKey, out var job))
        {
            job.Status = JobStatus.Completed;
            job.ResultJson = resultJson;
            job.CompletedAtUtc = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task SetFailedAsync(string idempotencyKey, string error, CancellationToken ct)
    {
        if (_jobs.TryGetValue(idempotencyKey, out var job))
        {
            job.Status = JobStatus.Failed;
            job.Error = error;
            job.CompletedAtUtc = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }
}
