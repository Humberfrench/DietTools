// =============================================================
// 5) JobWorkerGeneric.cs (BackgroundService)
// Namespace: Dietcode.Core.Jobs
// =============================================================
using Dietcode.Core.Jobs.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dietcode.Core.Jobs
{
    /// <summary>
    /// Worker genérico: fica consumindo IJobQueue e despacha para o handler correto.
    /// Por enquanto, ele trata GenericJob; você pode expandir depois para outros jobs.
    /// </summary>
    public sealed class JobWorkerGeneric : BackgroundService
    {
        private readonly IJobQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<JobWorkerGeneric> _logger;

        public JobWorkerGeneric(IJobQueue queue, IServiceScopeFactory scopeFactory, ILogger<JobWorkerGeneric> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ JobWorkerGeneric iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                IJob job;

                try
                {
                    job = await _queue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao dequeuar job");
                    continue;
                }

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    // Hoje só precisamos do GenericJob.
                    if (job is GenericJob generic)
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<IJobHandler<GenericJob>>();
                        await handler.HandleAsync(generic, stoppingToken);
                        continue;
                    }

                    _logger.LogWarning("Job desconhecido: {JobName} ({Id})", job.JobName, job.IdempotencyKey);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao processar job {JobName} ({Id})", job.JobName, job.IdempotencyKey);
                }
            }

            _logger.LogInformation("🛑 JobWorkerGeneric finalizado");
        }
    }
}