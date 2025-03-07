using Credpay.Tools.Library;
using static Credpay.Tools.Library.AppSettings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dietcode.Database.Net.Orm.Context
{
    public partial class ThisDatabase : DbContext
    {
        public ThisDatabase()
            : base("Name=ThisDatabaseContext")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            ConnectionString = GetConnString();

        }

        static ThisDatabase()
        {
            // Defina o inicializador do banco de dados aqui
            Database.SetInitializer<ThisDatabase>(null);
        }

        string GetConnString()
        {
            return Get("DbContextConnString");

        }

        public string ConnectionString { get; private set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }
}
