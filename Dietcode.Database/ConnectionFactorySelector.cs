using Dietcode.Database.DatabaseProviders;
using Dietcode.Database.Enums;
using Dietcode.Database.Interfaces;

namespace Dietcode.Database
{
    public static class ConnectionFactorySelector
    {
        public static IConnectionFactory Create(string cs, EnumBancos banco)
        {
            return banco switch
            {
                EnumBancos.SqlServer => new SqlConnectionFactory(cs),
                EnumBancos.MySql => new MySqlConnectionFactory(cs),
                EnumBancos.PostgreSql => new PostgreSqlConnectionFactory(cs),
                EnumBancos.Oracle => new OracleConnectionFactory(cs),
                _ => new SqlConnectionFactory(cs),
            };
        }
    }

}
