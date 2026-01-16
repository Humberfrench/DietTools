using Serilog;

namespace Dietcode.Database.Orm.Logging
{
    internal class EfPerformanceObserver : IObserver<KeyValuePair<string, object?>>
    {
        public void OnNext(KeyValuePair<string, object?> ev)
        {
            if (ev.Value == null)
            {
                return;
            }

            if (ev.Key == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")
            {
                dynamic data = ev.Value;

                Log.Information("EF Query Executed {@QueryInfo}",
                    new
                    {
                        data.Command.CommandText,
                        Duration = data.Duration.TotalMilliseconds,
                        data.Command.Parameters,
                        Method = data.ExecuteMethod.ToString()
                    });
            }
        }

        public void OnError(Exception error)
        {
            Log.Error(error, "EF performance observer error");
        }

        public void OnCompleted()
        {
            // No-op
        }
    }
}
