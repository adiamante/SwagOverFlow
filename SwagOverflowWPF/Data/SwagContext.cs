using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using SwagOverflowWPF.ViewModels;
using System;

namespace SwagOverflowWPF.Data
{
    public class SwagContext : DbContext
    {
        string _dataSource = "localhost";
        public DbSet<SwagGroupViewModel> SwagGroups { get; set; }
        public DbSet<SwagItemViewModel> SwagItems { get; set; }
        public DbSet<SwagSettingViewModel> SwatSettings { get; set; }
        public DbSet<SwagSettingGroupViewModel> SwatSettingGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SwagGroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagItemEntityConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string migrationConnectionString = $"Data Source = { _dataSource }; Initial Catalog = SwagOverflow; Integrated Security = True";
            optionsBuilder.UseSqlServer(migrationConnectionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    public class SwagGroupEntityConfiguration : IEntityTypeConfiguration<SwagGroupViewModel>
    {
        public void Configure(EntityTypeBuilder<SwagGroupViewModel> builder)
        {
            //SwagGroupViewModel GroupId => Key
            builder.HasKey(sg => sg.GroupId);

            //SwagGroupViewModel AlternateId => Unique
            builder.HasIndex(sg => sg.AlternateId).IsUnique();

            //SwagGroupViewModel One to One => SwagItemViewModel
            builder.HasOne(sg => sg.Root)
                .WithOne(si => si.GroupRoot)
                .HasForeignKey<SwagItemViewModel>(si => si.GroupRootId)
                .OnDelete(DeleteBehavior.NoAction);

            //SwagGroupViewModel Many to One => SwagItemViewModel
            builder.HasMany(sg => sg.Descendants)
                .WithOne(si => si.Group)
                .HasForeignKey(si => si.GroupId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class SwagItemEntityConfiguration : IEntityTypeConfiguration<SwagItemViewModel>
    {
        public void Configure(EntityTypeBuilder<SwagItemViewModel> builder)
        {
            //SwagItemViewModel Key + Id (AutoIncrement) => Key
            builder.HasKey(si => new { si.GroupId, si.ItemId });
            builder.Property(si => si.ItemId).ValueGeneratedOnAdd();

            //SwagItemViewModel AlternateId => Unique
            builder.HasIndex(si => si.AlternateId).IsUnique();

            //SwagItemViewModel Children =>  One to many
            builder.HasMany(si => si.Children)
                .WithOne(si => si.Parent)
                .HasForeignKey(si => new { si.GroupId, si.ParentId })
                .OnDelete(DeleteBehavior.NoAction);

            //SwagItemViewModel Value =>  Ignore
            builder.Ignore(si => si.Value);

            //Instruction SchemaCheck
            builder.Property(si => si.ValueType)
                .HasConversion(
                    si => JsonConvert.SerializeObject(si),
                    si => JsonConvert.DeserializeObject<Type>(si));
        }
    }
}
