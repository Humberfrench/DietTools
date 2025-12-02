using Dietcode.Database.Orm.Config;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
namespace Dietcode.Database.Orm.Logging
{
    internal static class InternalOrmLoggerFactory
    {
        private static readonly ILoggerFactory _factory;

        static InternalOrmLoggerFactory()
        {
            if (!OrmSettings.EnableLogging)
            {
                _factory = LoggerFactory.Create(builder => { });
                return;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    "logs/orm-log-.json",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    shared: true
                )
                .WriteTo.Console()
                .CreateLogger();

            _factory = new SerilogLoggerFactory(Log.Logger, dispose: false);
        }

        public static ILoggerFactory Instance => _factory;
    }
}
