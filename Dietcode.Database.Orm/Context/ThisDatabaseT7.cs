using Microsoft.EntityFrameworkCore;

namespace Dietcode.Database.Orm.Context
{
    public class ThisDatabase<T1, T2, T3, T4, T5, T6, T7> : ThisDatabase where T1 : class where T2 : class
                                                                 where T3 : class where T4 : class
                                                                 where T5 : class where T6 : class
                                                                 where T7 : class
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
        public virtual DbSet<T3> TableData3 { get; set; }
        public virtual DbSet<T4> TableData4 { get; set; }
        public virtual DbSet<T5> TableData5 { get; set; }
        public virtual DbSet<T6> TableData6 { get; set; }
        public virtual DbSet<T7> TableData7 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T1>();
            modelBuilder.Entity<T2>();
            modelBuilder.Entity<T3>();
            modelBuilder.Entity<T4>();
            modelBuilder.Entity<T5>();
            modelBuilder.Entity<T6>();
            modelBuilder.Entity<T7>();
            base.OnModelCreating(modelBuilder);

        }
    }
}
