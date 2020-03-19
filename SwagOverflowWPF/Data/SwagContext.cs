using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Utilities;
using SwagOverflowWPF.ViewModels;
using System;
using System.Data;
using System.Reflection;

namespace SwagOverflowWPF.Data
{
    public class SwagContext : DbContext
    {
        static string _dataSource = "localhost";
        public DbSet<SwagGroupViewModel> SwagGroups { get; set; }
        public DbSet<SwagItemViewModel> SwagItems { get; set; }
        public DbSet<SwagSettingViewModel> SwagSettings { get; set; }
        public DbSet<SwagSettingGroupViewModel> SwagSettingGroups { get; set; }
        public DbSet<SwagWindowSettingGroup> SwagWindowSettingGroups { get; set; }
        public DbSet<SwagDataRow> SwagDataRows { get; set; }
        public DbSet<SwagDataTable> SwagDataTables { get; set; }
        public SwagContext() : base() { }

        public SwagContext(DbContextOptions<SwagContext> options) : base (options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SwagGroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingViewModelEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataTableEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataRowEntityConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SetSqlServerOptions(optionsBuilder);
            //SetSqliteOptions(optionsBuilder);
        }

        public static void SetSqlServerOptions(DbContextOptionsBuilder optionsBuilder)
        {
            string migrationConnectionString = $"Data Source = { _dataSource }; Initial Catalog = SwagOverflow; Integrated Security = True";
            optionsBuilder.UseSqlServer(migrationConnectionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public static void SetSqliteOptions(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Data Source=settings.db";
            optionsBuilder.UseSqlite(connectionString);
        }

        public override void Dispose()
        {
            base.Dispose();
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
            //Group and GroupRoot exist in SwagItemViewModel as a workaround for circular references
            //Item.Group => Group.Root => Item.Group => (multiple Item.Group exist that have references to the same Group.Root) 
            //vs Item.GroupRoot => Group.Root => (limmited to one Item.GroupRoot)
            builder.HasOne(sg => sg.Root)
                .WithOne(si => si.GroupRoot)
                .HasForeignKey<SwagItemViewModel>(si => si.GroupRootId)
                .OnDelete(DeleteBehavior.NoAction);

            //SwagGroupViewModel Many to One => SwagItemViewModel
            //builder.HasMany(sg => sg.Descendants)
            //    .WithOne(si => si.Group)
            //    .HasForeignKey(si => si.GroupId)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class SwagItemEntityConfiguration : IEntityTypeConfiguration<SwagItemViewModel>
    {
        public void Configure(EntityTypeBuilder<SwagItemViewModel> builder)
        {
            //SwagItemViewModel Key + Id (AutoIncrement) => Key
            //Sqlite does not play too well with composite keys
            //builder.HasKey(si => new { si.GroupId, si.ItemId });
            builder.HasKey(si => si.ItemId);
            builder.Property(si => si.ItemId).ValueGeneratedOnAdd();

            //SwagItemViewModel AlternateId => Unique
            builder.HasIndex(si => si.AlternateId).IsUnique();

            //SwagItemViewModel Children =>  One to many
            //Sqlite does not play too well with composite keys
            //builder.HasMany(si => si.Children)
            //    .WithOne(si => si.Parent)
            //    .HasForeignKey(si => new { si.GroupId, si.ParentId })
            //    .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(si => si.Children)
                .WithOne(si => si.Parent)
                .HasForeignKey(si => si.ParentId)
                .OnDelete(DeleteBehavior.NoAction);

            //SwagItemViewModel Value
            builder.Property(si => si.Value)
                .HasConversion(
                    si => JsonHelper.ToJsonString(si, ShouldSerializeContractResolver.Instance),
                    si => JsonConvert.DeserializeObject<object>(si));

            //SwagItemViewModel ValueType => Ignore
            builder.Ignore(si => si.ValueType);

            //SwagItemViewModel One to One => SwagGroupViewModel
            //builder.HasOne(si => si.GroupRoot)
            //    .WithOne(sg => sg.Root)
            //    .HasForeignKey<SwagGroupViewModel>(sg => new { sg.GroupId, sg.RootId })
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class SwagSettingViewModelEntityConfiguration : IEntityTypeConfiguration<SwagSettingViewModel>
    {
        public void Configure(EntityTypeBuilder<SwagSettingViewModel> builder)
        {
            //SwagSettingViewModel SettingType => Convert to String
            EnumToStringConverter<SettingType> settingTypeconverter = new EnumToStringConverter<SettingType>();
            builder.Property(ss => ss.SettingType).HasConversion(settingTypeconverter);

            //SwagSettingViewModel ItemsSource => Convert to String
            builder.Property(ss => ss.ItemsSource)
                .HasConversion(
                    ss => JsonHelper.ToJsonString(ss),
                    ss => JsonConvert.DeserializeObject<object>(ss));
        }
    }


    public class SwagDataTableEntityConfiguration : IEntityTypeConfiguration<SwagDataTable>
    {
        public void Configure(EntityTypeBuilder<SwagDataTable> builder)
        {
            //SwagDataTable DataTable => Ignore
            builder.Ignore(sdt => sdt.DataTable);
        }
    }

    public class SwagDataRowEntityConfiguration : IEntityTypeConfiguration<SwagDataRow>
    {
        public void Configure(EntityTypeBuilder<SwagDataRow> builder)
        {
            //SwagDataRow DataRow => Ignore
            builder.Ignore(sdr => sdr.DataRow);
        }
    }

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public new static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if ((property.DeclaringType == typeof(DataRow) && property.PropertyName == "Table") ||
                property.DeclaringType == typeof(DataColumn) && property.PropertyName == "Table")
            {
                property.ShouldSerialize = i => false;
            }

            return property;
        }
    }
}
