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
