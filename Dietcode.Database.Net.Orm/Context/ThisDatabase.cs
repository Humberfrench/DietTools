using System.Data.Entity;
using static Credpay.Tools.Library.AppSettings;


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
            this.Database.Initialize(false);
        }

        //static ThisDatabase()
        //{
        //    // Defina o inicializador do banco de dados aqui
        //    //Database.SetInitializer<ThisDatabase>(null);
        //}

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
