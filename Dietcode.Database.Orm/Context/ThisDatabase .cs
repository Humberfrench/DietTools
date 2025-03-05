using Dietcode.Core.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace Dietcode.Database.Orm.Context
{
    public abstract class ThisDatabase : DbContext
    {
        protected ThisDatabase()
        {
            ConnectionString = GetConnString();
        }

        public ThisDatabase(DbContextOptions<ThisDatabase> options)
            : base(options)
        {
            ConnectionString = GetConnString();
        }

        string GetConnString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                       .SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile("appsettings.json")
                                       .Build();
            return configuration.GetConnectionString("DbContextConnString")!;

        }

        public string ConnectionString { get; private set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (ConnectionString.IsNullOrEmptyOrWhiteSpace())
            {
                throw new ArgumentException("Connection String Inválida");
            }
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString);

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
