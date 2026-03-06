namespace Dietcode.Core.Jobs.Interfaces.Domain
{
    // =============================================================
    // 2) AsyncJobStateGeneric.cs
    // Namespace: Dietcode.Core.Jobs.Interfaces.Domain
    // =============================================================

    /// <summary>
    /// Estado persistido do job genérico.
    /// Obs: payload/result como JSON deixam o pipeline independente do tipo.
    /// </summary>
    public sealed class AsyncJobStateGeneric
    {
        public string IdempotencyKey { get; set; } = default!;
        public string HandlerKey { get; set; } = default!;
        public string PayloadJson { get; set; } = default!;
        public JobStatus Status { get; set; } = JobStatus.Processing;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAtUtc { get; set; }

        public string? ResultJson { get; set; }
        public string? Error { get; set; }
    }

}
