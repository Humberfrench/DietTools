using Dietcode.Core.Jobs.Interfaces.Domain;

namespace Dietcode.Core.Jobs.Interfaces
{
    // =============================================================
    // 1) IAsyncJobStoreGeneric.cs
    // Namespace: Dietcode.Core.Jobs.Interfaces
    // =============================================================
    /// <summary>
    /// Store genérico: persiste o estado do job assíncrono (handler + payload + resultado).
    /// Implementações típicas: InMemory, Redis, Mongo, SQL.
    /// </summary>
    public interface IAsyncJobStoreGeneric
    {
        Task CreateAsync(AsyncJobStateGeneric job, CancellationToken ct);
        Task<AsyncJobStateGeneric?> GetAsync(string idempotencyKey, CancellationToken ct);
        Task SetCompletedAsync(string idempotencyKey, string resultJson, CancellationToken ct);
        Task SetFailedAsync(string idempotencyKey, string error, CancellationToken ct);
    }
}
