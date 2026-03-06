using Dietcode.Api.Core.Results;
using Dietcode.Core.Jobs.Interfaces.Domain;

namespace Dietcode.Core.Jobs.Interfaces
{
    public interface IJobAsyncService<TRequest, TResult>
    {
        Task<MethodResult<AsyncReturn>> StartAsync(AsyncStartRequest<TRequest> request, CancellationToken ct);
        Task<MethodResult<AsyncReturn>> GetStatusAsync(string idempotencyKey, CancellationToken ct);
        Task<MethodResult<TResult>> GetResultAsync(string idempotencyKey, CancellationToken ct);
    }
}
