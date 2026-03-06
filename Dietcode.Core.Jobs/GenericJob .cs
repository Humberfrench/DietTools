using Dietcode.Core.Jobs.Interfaces;

namespace Dietcode.Core.Jobs
{
    public sealed class GenericJob : IJob
    {
        public string JobId { get; }     // pode ser o próprio id
        public string JobName => nameof(GenericJob);

        public string IdempotencyKey { get; }

        public GenericJob(string idempotencyKey)
        {
            IdempotencyKey = idempotencyKey;
            JobId = idempotencyKey;
        }
    }
}
