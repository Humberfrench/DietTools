namespace Dietcode.Core.Jobs.Interfaces
{
    public interface IJob
    {
        // Marcador: um "tipo" que representa um trabalho enfileirável.
        string IdempotencyKey { get; }
        string JobName { get; }
    }
}
