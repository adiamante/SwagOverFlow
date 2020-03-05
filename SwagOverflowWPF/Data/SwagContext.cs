using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Data
{
    public class SwagContext : DbContext
    {
        string _dataSource = "localhost";
        public DbSet<SwagItemViewModel> SwagItems { get; set; }
        public DbSet<SwagSettingViewModel> SwatSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SwagItemEntityConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string migrationConnectionString = $"Data Source = { _dataSource }; Initial Catalog = SwagOverflow; Integrated Security = True";
            optionsBuilder.UseSqlServer(migrationConnectionString);
        }
    }

    public class SwagItemEntityConfiguration : IEntityTypeConfiguration<SwagItemViewModel>
    {
        public void Configure(EntityTypeBuilder<SwagItemViewModel> builder)
        {
            //SwagItemViewModel Key + Id (AutoIncrement) => Key
            builder.HasKey(si => new { si.Group, si.Key, si.Id });
            builder.Property(si => si.Id).ValueGeneratedOnAdd();

            //SwagItemViewModel Children =>  One to many
            builder.HasMany(si => si.Children)
                .WithOne(si => si.Parent)
                .HasForeignKey(si => new { si.Group, si.Key, si.ParentId })
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
