using Dietcode.Core.Jobs.Interfaces;

namespace Dietcode.Core.Jobs
{
    public sealed class GenericJob(string idempotencyKey) : IJob
    {
        public string JobId { get; } = idempotencyKey;
        public string JobName => nameof(GenericJob);

        public string IdempotencyKey { get; } = idempotencyKey;
    }
}
