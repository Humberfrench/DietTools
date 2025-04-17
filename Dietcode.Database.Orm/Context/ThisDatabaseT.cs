using Microsoft.EntityFrameworkCore;

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
}
