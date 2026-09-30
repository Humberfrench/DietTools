namespace Dietcode.Core.Jobs.Redis;

public sealed class RedisJobOptions
{
    public string ConnectionString { get; set; } = "localhost:6379";

    public int? Database { get; set; }

    public string QueueKey { get; set; } = "dietcode:jobs:queue";

    public string StoreKeyPrefix { get; set; } = "dietcode:jobs:state:";

    /// <summary>
    /// Timeout (em segundos) de cada BRPOP. O worker fica bloqueado no Redis por até esse
    /// tempo esperando um job; se nada chegar, tenta de novo. Também é a latência máxima
    /// de cancelamento do DequeueAsync, já que o StackExchange.Redis não cancela um
    /// comando em voo.
    /// </summary>
    public int DequeuePollingSeconds { get; set; } = 5;

    /// <summary>
    /// TTL opcional do estado do job no Redis. Nulo (padrão) = sem expiração, igual a um
    /// dicionário em memória guardaria para sempre.
    /// </summary>
    public TimeSpan? StateTimeToLive { get; set; }

    internal IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ConnectionString))
            errors.Add("Redis ConnectionString obrigatorio.");

        if (string.IsNullOrWhiteSpace(QueueKey))
            errors.Add("Redis QueueKey obrigatorio.");

        if (string.IsNullOrWhiteSpace(StoreKeyPrefix))
            errors.Add("Redis StoreKeyPrefix obrigatorio.");

        if (DequeuePollingSeconds <= 0)
            errors.Add("Redis DequeuePollingSeconds deve ser maior que zero.");

        return errors;
    }
}
