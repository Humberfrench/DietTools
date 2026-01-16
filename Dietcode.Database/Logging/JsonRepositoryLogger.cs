using Dietcode.Core.Lib.Masking;
using Dietcode.Database.Abstractions;
using System.Text.Json;

namespace Dietcode.Database.Logging
{
    public sealed class JsonRepositoryLogger : IRepositoryLogger
    {
        public async Task LogAsync(
            string operation,
            object? context,
            TimeSpan duration,
            Exception? exception = null)
        {
            var entry = new
            {
                Timestamp = DateTimeOffset.UtcNow,
                Operation = operation,
                Context = SensitiveDataMasker.Mask(context),
                DurationMs = duration.TotalMilliseconds,
                Error = exception?.Message
            };

            var json = JsonSerializer.Serialize(entry, JsonLogOptions.Default);
            await File.AppendAllTextAsync(
                "db-log.jsonl",
                json + Environment.NewLine);
        }
    }
}
