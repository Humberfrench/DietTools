using System.Data.Entity;

namespace Dietcode.Database.Net.Orm.Context
{
    public class ThisDatabase<Table> : ThisDatabase where Table : class
    {
        public ThisDatabase() : base()
        {
        }

        public virtual DbSet<Table> TableData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
