using Microsoft.EntityFrameworkCore;

namespace Dietcode.Database.Orm.Context
{
    public class ThisDatabase<T1, T2> : ThisDatabase where T1 : class where T2 : class
    {
        public ThisDatabase() : base()
        {
        }

        public ThisDatabase(DbContextOptions<ThisDatabase> options)
            : base(options)
        {

        }

        public virtual DbSet<T1> TableData1 { get; set; }
        public virtual DbSet<T2> TableData2 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T1>();
            modelBuilder.Entity<T2>();
            base.OnModelCreating(modelBuilder);

        }

    }
}
