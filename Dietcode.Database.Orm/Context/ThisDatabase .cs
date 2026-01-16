using Dietcode.Core.Lib;
using Dietcode.Database.Orm.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Dietcode.Database.Orm.Context
{
    public class ThisDatabase<T1> : ThisDatabase where T1 : class
    {
        public ThisDatabase() : base()
        {
        }

        public ThisDatabase(DbContextOptions<ThisDatabase> options)
            : base(options)
        {
        }

        public virtual DbSet<T1> TableData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T1>();
            base.OnModelCreating(modelBuilder);
        }

    }

    public abstract class ThisDatabase : DbContext
    {
        protected ThisDatabase()
        {
            ConnectionString = GetConnString();
        }

        protected ThisDatabase(DbContextOptions<ThisDatabase> options)
            : base(options)
        {
            ConnectionString = GetConnString();
        }

        private readonly DiagnosticListener listener = new("ORM.Database.Listener");

        public string ConnectionString { get; private set; }

        private static string GetConnString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration.GetConnectionString("DbContextConnString")
                   ?? throw new ArgumentException("Connection String Inválida");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (ConnectionString.IsNullOrEmptyOrWhiteSpace())
            {
                throw new ArgumentException("Connection String Inválida");
            }

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(ConnectionString)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .EnableServiceProviderCaching()
                    .UseLoggerFactory(InternalOrmLoggerFactory.Instance)
                    .LogTo(Console.WriteLine,
                           Microsoft.Extensions.Logging.LogLevel.Information,
                           Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.Category
                           | Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.SingleLine
                           | Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.UtcTime);

                listener.Subscribe(new EfPerformanceObserver());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

