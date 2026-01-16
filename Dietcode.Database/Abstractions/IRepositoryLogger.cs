namespace Dietcode.Database.Abstractions
{
    public interface IRepositoryLogger
    {
        Task LogAsync(
            string operation,
            object? context,
            TimeSpan duration,
            Exception? exception = null);
    }
}
