// =============================================================
// 4) GenericJobHandler.cs (IJobHandler<GenericJob>)
// Namespace: Dietcode.Core.Jobs
// =============================================================
using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Interfaces.Domain;

namespace Dietcode.Core.Jobs
{
    /// <summary>
    /// Handler do job genérico: lê o estado no store, dispara o handler pelo HandlerKey,
    /// persiste o resultado e marca Completed/Failed.
    /// </summary>
    public sealed class GenericJobHandler(IAsyncJobStoreGeneric store, IHandlerDispatcher dispatcher) : IJobHandler<GenericJob>
    {
        private readonly IAsyncJobStoreGeneric _store = store;
        private readonly IHandlerDispatcher _dispatcher = dispatcher;

        public async Task HandleAsync(GenericJob job, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var state = await _store.GetAsync(job.IdempotencyKey, ct);
            if (state is null)
            {
                // Não existe no store: nada a fazer.
                return;
            }

            // Se já terminou, não reprocessa.
            if (state.Status == JobStatus.Completed || state.Status == JobStatus.Failed)
                return;

            try
            {
                ct.ThrowIfCancellationRequested();

                var resultJson = await _dispatcher.ExecuteAsync(state.HandlerKey, state.PayloadJson, ct);

                ct.ThrowIfCancellationRequested();

                await _store.SetCompletedAsync(state.IdempotencyKey, resultJson, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Cancelamento: deixa subir para o worker decidir.
                throw;
            }
            catch (Exception ex)
            {
                await _store.SetFailedAsync(state.IdempotencyKey, ex.ToString(), ct);
            }
        }
    }
}

