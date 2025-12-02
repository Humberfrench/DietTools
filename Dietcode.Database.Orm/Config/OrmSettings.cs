using Microsoft.Extensions.Configuration;
using System.IO;

namespace Dietcode.Database.Orm.Config
{
    internal static class OrmSettings
    {
        public static bool EnableLogging { get; private set; } = true;

        static OrmSettings()
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();

                var value = config["Orm:EnableLogging"];

                if (!string.IsNullOrWhiteSpace(value) &&
                    bool.TryParse(value, out bool parsed))
                {
                    EnableLogging = parsed;
                }
            }
            catch
            {
                // Se falhar, mantém logging habilitado
                EnableLogging = true;
            }
        }
    }
}
