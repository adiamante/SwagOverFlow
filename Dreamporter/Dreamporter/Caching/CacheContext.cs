using Microsoft.EntityFrameworkCore;

namespace Dreamporter.Caching
{
    public class UtilContext : DbContext
    {
        public DbSet<CacheRecord> CacheRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string connectionString = "Data Source=util.db";
            //optionsBuilder.UseSqlite(connectionString);
        }

        public UtilContext() : base()
        {

        }

        public UtilContext(DbContextOptions options) : base(options)
        {

        }
    }
}
