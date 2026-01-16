using Dietcode.Database.Abstractions;
using System.Diagnostics;

namespace Dietcode.Database.Logging
{
    public sealed class LoggingRepositoryDecorator<T>
        : RepositoryDecorator<T>
        where T : class
    {
        private readonly IRepositoryLogger _logger;

        public LoggingRepositoryDecorator(
            IRepository<T> inner,
            IRepositoryLogger logger)
            : base(inner)
        {
            _logger = logger;
        }

        public override async Task<T?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var result = await base.GetByIdAsync(id, cancellationToken);
                sw.Stop();

                await _logger.LogAsync(
                    operation: "GetById",
                    context: new { id },
                    duration: sw.Elapsed);

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();

                await _logger.LogAsync(
                    operation: "GetById",
                    context: new { id },
                    duration: sw.Elapsed,
                    exception: ex);

                throw;
            }
        }
    }
}
