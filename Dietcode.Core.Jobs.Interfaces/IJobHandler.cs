// =============================================================
// 6) (OPCIONAL) Interface IJobHandler<TJob>
// Namespace: Dietcode.Core.Jobs.Interfaces
// =============================================================
namespace Dietcode.Core.Jobs.Interfaces
{
    public interface IJobHandler<in TJob> where TJob : IJob
    {
        Task HandleAsync(TJob job, CancellationToken ct);
    }
}

