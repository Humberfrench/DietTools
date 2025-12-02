using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dietcode.Database.Orm.Logging
{
    internal class EfPerformanceObserver : IObserver<KeyValuePair<string, object>>
    {
        public void OnNext(KeyValuePair<string, object> ev)
        {
            if (ev.Key == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")
            {
                dynamic data = ev.Value;

                Log.Information("EF Query Executed {@QueryInfo}",
                    new
                    {
                        CommandText = data.Command.CommandText,
                        Duration = data.Duration.TotalMilliseconds,
                        Parameters = data.Command.Parameters,
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
