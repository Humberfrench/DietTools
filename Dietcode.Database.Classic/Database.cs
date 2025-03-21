using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace Dietcode.Database.Classic
{
    public abstract class Database : DbContext
    {
        public const decimal Version = 0.8m;

        private static SQLVersion version = SQLVersion.SQL2019;
        private static string? defaultConnectionString = null;
        private string? connectionString;

        protected Database()
        {
            connectionString = defaultConnectionString;
        }

        protected Database(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.EnableSensitiveDataLogging(true);

                switch (version)
                {
                    case SQLVersion.SQL2012:
                        {
                            optionsBuilder.UseSqlServer(connectionString, o => {
                                //o.UseRowNumberForPaging();
                                o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                            });
                            break;
                        }
                    case SQLVersion.SQL2016:
                    case SQLVersion.SQL2019:
                    default:
                        {
                            optionsBuilder.UseSqlServer(connectionString, o => {
                                o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                            });
                            break;
                        }
                }
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 8);

            base.ConfigureConventions(configurationBuilder);
        }

        public static void Configure(string connectionString, SQLVersion version = SQLVersion.SQL2016)
        {
            defaultConnectionString = connectionString;
            Classic.Database.version = version;
        }

        public enum SQLVersion
        {
            SQL2019 = 0,
            SQL2016 = 1,
            SQL2012 = 2,
        }
    }
}
