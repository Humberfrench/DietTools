namespace Dietcode.Core.Jobs.Interfaces.Domain
{
    /// <summary>
    /// Job genérico: carrega apenas o idempotencyKey.
    /// O worker busca detalhes (handlerKey/payload) no store.
    /// </summary>
    public sealed class GenericJob : IJob
    {
        public string IdempotencyKey { get; }
        public string JobName => nameof(GenericJob);


        public GenericJob(string idempotencyKey)
        {
            IdempotencyKey = idempotencyKey;
        }
    }
}
