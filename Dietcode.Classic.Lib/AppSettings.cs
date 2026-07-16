using System.Configuration;

namespace Dietcode.Classic.Lib
{
    public static class AppSettings
    {
        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static Uri GetUri(string key)
        {
            var keyString = ConfigurationManager.AppSettings[key];
            var uri = new Uri(keyString);
            return uri;
        }

        public static Guid GetGuid(string key)
        {
            var keyString = ConfigurationManager.AppSettings[key];
            var guid = new Guid(keyString);
            return guid;
        }

        public static bool GetBoolean(string key)
        {
            var valor = ConfigurationManager.AppSettings[key];
            if (valor.IsNullOrEmptyOrWhiteSpace())
            {
                return false;
            }
            if (string.Equals(valor, "true", System.StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public static int GetInt(string key)
        {

            var valor = ConfigurationManager.AppSettings[key];

            int.TryParse(valor, out var retorno);

            return retorno;

        }

        public static long GetLong(string key)
        {

            var valor = ConfigurationManager.AppSettings[key];

            long.TryParse(valor, out var retorno);

            return retorno;

        }

        public static byte GetByte(string key)
        {

            var valor = ConfigurationManager.AppSettings[key];

            byte.TryParse(valor, out var retorno);

            return retorno;

        }

        public static string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}