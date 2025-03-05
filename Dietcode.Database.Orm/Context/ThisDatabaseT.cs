using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database.Orm.Context
{
    public class ThisDatabase<Table> : ThisDatabase where Table : class
    {
        public ThisDatabase() : base()
        {
        }

        public ThisDatabase(DbContextOptions<ThisDatabase> options)
            : base(options)
        {

        }

        public virtual DbSet<Table> TableData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>();
            base.OnModelCreating(modelBuilder);

        }

    }
}
