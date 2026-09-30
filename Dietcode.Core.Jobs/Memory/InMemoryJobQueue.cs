using System.Threading.Channels;
using Dietcode.Core.Jobs.Interfaces;

namespace Dietcode.Core.Jobs.Memory;

/// <summary>
/// IJobQueue em memória via Channel&lt;IJob&gt;: fila FIFO local ao processo, não
/// sobrevive a restart nem é compartilhada entre instâncias da aplicação. É o provider
/// padrão — igual a qualquer outro (ex.: Redis), implementa o mesmo contrato IJobQueue,
/// sem tratamento especial em JobAsyncService/JobWorkerGeneric.
/// </summary>
public sealed class InMemoryJobQueue : IJobQueue
{
    private readonly Channel<IJob> _channel = Channel.CreateUnbounded<IJob>();

    public ValueTask EnqueueAsync(IJob job, CancellationToken ct)
        => _channel.Writer.WriteAsync(job, ct);

    public async ValueTask<IJob> DequeueAsync(CancellationToken ct)
        => await _channel.Reader.ReadAsync(ct);
}
