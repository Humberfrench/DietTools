using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dietcode.Database.Classic
{
    public class Database<T> : Database where T : class
    {
        public Database() : base()
        { }

        public Database(string connectionString) : base(connectionString)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T>();
        }
    }
}
