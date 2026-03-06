using System.Text.Json;
using Dietcode.Api.Core.Results;
using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Interfaces.Domain;
namespace Dietcode.Core.Jobs
{

    public sealed class JobAsyncService<TRequest, TResult> : AppServiceBase, IJobAsyncService<TRequest, TResult>
    {
        private readonly IAsyncJobStoreGeneric _store;
        private readonly IJobQueue _queue;

        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public JobAsyncService(IAsyncJobStoreGeneric store, IJobQueue queue)
        {
            _store = store;
            _queue = queue;
        }

        public async Task<MethodResult<AsyncReturn>> StartAsync(AsyncStartRequest<TRequest> request, CancellationToken ct)
        {
            if (request is null)
                return BadRequest("Request inválido.", new AsyncReturn { Status = JobStatus.Failed, StatusCode = ResultStatusCode.BadRequest });

            if (string.IsNullOrWhiteSpace(request.HandlerKey))
                return BadRequest("HandlerKey obrigatório.", new AsyncReturn { Status = JobStatus.Failed, StatusCode = ResultStatusCode.BadRequest });

            var id = Guid.NewGuid().ToString("D");

            var job = new AsyncJobStateGeneric
            {
                IdempotencyKey = id,
                HandlerKey = request.HandlerKey,
                PayloadJson = JsonSerializer.Serialize(request.Payload, JsonOpts),
                Status = JobStatus.Processing
            };

            await _store.CreateAsync(job, ct);
            await _queue.EnqueueAsync(new GenericJob(id), ct);

            var asyncReturn = new AsyncReturn
            {
                IdempotencyKey = id,
                Status = JobStatus.Processing,
                StatusCode = ResultStatusCode.Accepted
            };

            return Accepted(asyncReturn, id);
        }

        public async Task<MethodResult<AsyncReturn>> GetStatusAsync(string idempotencyKey, CancellationToken ct)
        {
            var job = await _store.GetAsync(idempotencyKey, ct);
            if (job is null)
            {
                var notFound = new AsyncReturn
                {
                    IdempotencyKey = idempotencyKey,
                    Status = JobStatus.NotFound,
                    StatusCode = ResultStatusCode.NotFound
                };
                return BadRequest("Não encontrado", notFound);
            }

            var statusCode = job.Status switch
            {
                JobStatus.Processing => ResultStatusCode.Accepted,
                JobStatus.Completed => ResultStatusCode.OK,
                JobStatus.Failed => ResultStatusCode.InternalServerError,
                _ => ResultStatusCode.InternalServerError
            };

            var ret = new AsyncReturn
            {
                IdempotencyKey = job.IdempotencyKey,
                Status = job.Status,
                StatusCode = statusCode
            };

            return Ok(ret);
        }

        public async Task<MethodResult<TResult>> GetResultAsync(string idempotencyKey, CancellationToken ct)
        {
            var job = await _store.GetAsync(idempotencyKey, ct);
            if (job is null)
                return BadRequest("Não encontrado", default!);

            if (job.Status == JobStatus.Processing)
                return Accepted(default!, idempotencyKey);

            if (job.Status == JobStatus.Failed)
                return new ErrorResult<TResult>(
                    default!,
                    ResultStatusCode.InternalServerError,
                    new ErrorValidation { Code = "500", Message = job.Error ?? "Falha no job." });

            // Completed
            if (string.IsNullOrWhiteSpace(job.ResultJson))
                return Ok(default!);

            var result = JsonSerializer.Deserialize<TResult>(job.ResultJson, JsonOpts);
            return Ok(result!);
        }
    }
}
