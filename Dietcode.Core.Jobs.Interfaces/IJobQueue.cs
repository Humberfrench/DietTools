namespace Dietcode.Core.Jobs.Interfaces
{
    /// <summary>
    /// Fila de jobs genéricos. Pode ter provider Channel/Redis/etc.
    /// </summary>
    public interface IJobQueue
    {
        ValueTask EnqueueAsync(IJob job, CancellationToken ct);
        ValueTask<IJob> DequeueAsync(CancellationToken ct);
    }
}
