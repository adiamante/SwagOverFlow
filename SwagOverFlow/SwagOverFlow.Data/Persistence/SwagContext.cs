using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SwagOverFlow.ViewModels;
using SwagOverFlow.Utils;
using System;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Collections;

namespace SwagOverFlow.Data.Persistence
{
    public class SwagContext : DbContext
    {
        static string _dataSource = "localhost";
        public DbSet<SwagItemBase> SwagItems { get; set; }
        public DbSet<SwagValueItemBase> SwagValueItems { get; set; }
        public DbSet<SwagSetting> SwagSettings { get; set; }
        public DbSet<SwagSettingGroup> SwagSettingGroups { get; set; }
        public DbSet<SwagSettingString> SwagSettingStrings { get; set; }
        public DbSet<SwagSettingBoolean> SwagSettingBooleans { get; set; }
        public DbSet<SwagSettingInt> SwagSettingInts { get; set; }
        public DbSet<SwagData> SwagData { get; set; }
        public DbSet<SwagDataGroup> SwagDataGroups { get; set; }
        public DbSet<SwagDataSet> SwagDataSets { get; set; }
        public DbSet<SwagDataTable> SwagDataTables { get; set; }
        public DbSet<SwagDataColumn> SwagDataColumns { get; set; }
        public DbSet<SwagDataRow> SwagDataRows { get; set; }
        public SwagContext() : base() { }

        public SwagContext(DbContextOptions<SwagContext> options) : base (options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SwagItemBaseEntityConfiguration()); 
            modelBuilder.ApplyConfiguration(new SwagValueItemBaseEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingGroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingStringEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingBooleanEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingIntEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataGroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataTableEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataColumnEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataRowEntityConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //SetSqlServerOptions(optionsBuilder);
            SetSqliteOptions(optionsBuilder);
        }

        public static void SetSqlServerOptions(DbContextOptionsBuilder optionsBuilder)
        {
            string migrationConnectionString = $"Data Source = { _dataSource }; Initial Catalog = SwagOverFlow; Integrated Security = True";
            optionsBuilder.UseSqlServer(migrationConnectionString);
            optionsBuilder.EnableSensitiveDataLogging();
            
        }

        public static void SetSqliteOptions(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Data Source=settings.db";
            optionsBuilder.UseSqlite(connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public class SwagItemBaseEntityConfiguration : IEntityTypeConfiguration<SwagItemBase>
    {
        public void Configure(EntityTypeBuilder<SwagItemBase> builder)
        {
            //SwagItemBase Key + Id (AutoIncrement) => Key
            //Sqlite does not play too well with composite keys
            //builder.HasKey(si => new { si.GroupId, si.ItemId });
            builder.HasKey(si => si.ItemId);
            builder.Property(si => si.ItemId).ValueGeneratedOnAdd();

            //SwagItemBase AlternateId => Unique
            builder.HasIndex(si => si.AlternateId).IsUnique();
        }
    }

    public class SwagValueItemBaseEntityConfiguration : IEntityTypeConfiguration<SwagValueItemBase>
    {
        public void Configure(EntityTypeBuilder<SwagValueItemBase> builder)
        {
            //SwagItemBase Value
            builder.Property(si => si.ObjValue)
                .HasConversion(
                    si => JsonHelper.ToJsonString(si),
                    si => JsonConvert.DeserializeObject<object>(si));

            //SwagIndexedItemViewModel ValueType => Ignore
            builder.Ignore(si => si.ValueType);
        }
    }

    public class SwagSettingEntityConfiguration : IEntityTypeConfiguration<SwagSetting>
    {
        public void Configure(EntityTypeBuilder<SwagSetting> builder)
        {
            //SwagSetting SettingType => Convert to String
            EnumToStringConverter<SettingType> settingTypeconverter = new EnumToStringConverter<SettingType>();
            builder.Property(ss => ss.SettingType).HasConversion(settingTypeconverter);

            //SwagSetting Icon => Ignore
            builder.Ignore(ss => ss.Icon);
            builder.Ignore(ss => ss.Icon2);

            //SwagSetting Data => Convert to String
            builder.Property(ss => ss.Data)
                .HasConversion(
                    ss => JsonHelper.ToJsonString(ss),
                    ss => JsonConvert.DeserializeObject<JObject>(ss));
        }
    }

    public class SwagSettingGroupEntityConfiguration : IEntityTypeConfiguration<SwagSettingGroup>
    {
        public void Configure(EntityTypeBuilder<SwagSettingGroup> builder)
        {
            //SwagSetting Children =>  One to many
            builder.HasMany(ss => ss.Children)
                .WithOne(ss => ss.Parent)
                .HasForeignKey(ss => ss.ParentId)
                .OnDelete(DeleteBehavior.NoAction);

            //SwagSetting Icon => Ignore
            builder.Ignore(ss => ss.Icon);
            builder.Ignore(ss => ss.Icon2);
        }
    }

    public class SwagDataGroupEntityConfiguration : IEntityTypeConfiguration<SwagDataGroup>
    {
        public void Configure(EntityTypeBuilder<SwagDataGroup> builder)
        {
            //SwagSetting Children =>  One to many
            builder.HasMany(sdg => sdg.Children)
                .WithOne(sd => sd.Parent)
                .HasForeignKey(sd => sd.ParentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class SwagDataTableEntityConfiguration : IEntityTypeConfiguration<SwagDataTable>
    {
        public void Configure(EntityTypeBuilder<SwagDataTable> builder)
        {
            //SwagDataTable DataTable => Ignore
            builder.Ignore(sdt => sdt.DataTable);

            Func<String, SwagObservableOrderedDictionary<String, SwagDataColumn>> stringToSwagDict = (str) =>
            {
                SwagObservableOrderedDictionary<string, SwagDataColumn> dict = JsonConvert.DeserializeObject<SwagObservableOrderedDictionary<string, SwagDataColumn>>(str);
                return dict;
            };

            //SwagDataTable Columns
            builder.Property(sdt => sdt.Columns)
                .HasConversion(
                    sdc => JsonConvert.SerializeObject(sdc, Formatting.Indented),
                    sdc => stringToSwagDict(sdc)
             );

            //TODO: Fix
            //SwagDataTable Settings
            //builder.Property(si => si.Settings)
            //    .HasConversion(
            //        si => JsonHelper.ToJsonString(si),
            //        si => JsonHelper.ToObject<SwagSettingGroup>(si));

            //TODO: Fix
            //SwagDataTable Tabs
            //builder.Property(si => si.Tabs)
            //    .HasConversion(
            //        si => JsonHelper.ToJsonString(si),
            //        si => JsonHelper.ToObject<SwagTabGroup>(si));
        }
    }

    public class SwagDataColumnEntityConfiguration : IEntityTypeConfiguration<SwagDataColumn>
    {
        public void Configure(EntityTypeBuilder<SwagDataColumn> builder)
        {
            //SwagDataColumn SwagDataTable => Ignore
            builder.Ignore(sdc => sdc.SwagDataTable);
        }
    }

    public class SwagDataRowEntityConfiguration : IEntityTypeConfiguration<SwagDataRow>
    {
        public void Configure(EntityTypeBuilder<SwagDataRow> builder)
        {
            //SwagDataRow DataRow => Ignore
            builder.Ignore(sdr => sdr.DataRow);

            //SwagDataRow DataRow => Ignore
            builder.Ignore(sdr => sdr.Value);
        }
    }

    public class SwagSettingStringEntityConfiguration : IEntityTypeConfiguration<SwagSettingString>
    {
        public void Configure(EntityTypeBuilder<SwagSettingString> builder)
        {
            //SwagSettingString Value => Ignore
            builder.Ignore(ss => ss.Value);

            //SwagSettingString ItemsSource => Ignore
            builder.Ignore(ss => ss.ItemsSource);
        }
    }

    public class SwagSettingBooleanEntityConfiguration : IEntityTypeConfiguration<SwagSettingBoolean>
    {
        public void Configure(EntityTypeBuilder<SwagSettingBoolean> builder)
        {
            //SwagSettingBoolean Value => Ignore
            builder.Ignore(ss => ss.Value);

            //SwagSettingBoolean ItemsSource => Ignore
            builder.Ignore(ss => ss.ItemsSource);
        }
    }

    public class SwagSettingIntEntityConfiguration : IEntityTypeConfiguration<SwagSettingInt>
    {
        public void Configure(EntityTypeBuilder<SwagSettingInt> builder)
        {
            //SwagSettingInt Value => Ignore
            builder.Ignore(ss => ss.Value);

            //SwagSettingInt ItemsSource => Ignore
            builder.Ignore(ss => ss.ItemsSource);
        }
    }

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.Ignored == true ||
                (property.DeclaringType == typeof(DataRow) && property.PropertyName == "Table") ||
                (property.DeclaringType == typeof(DataColumn) && property.PropertyName == "Table") ||
                (property.DeclaringType == typeof(SwagDataColumn) && (property.PropertyName == "DataGridColumn" || property.PropertyName == "DataColumn")))
            {
                property.ShouldSerialize = i => false;
            }

            return property;
        }
    }

    //automatic Property json conversion
    //https://stackoverflow.com/questions/48449887/custom-type-with-automatic-serialization-deserialization-in-ef-core

}
