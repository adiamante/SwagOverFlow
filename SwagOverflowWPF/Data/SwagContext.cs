using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SwagOverflowWPF.Collections;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Utilities;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace SwagOverflowWPF.Data
{
    public class SwagContext : DbContext
    {
        static string _dataSource = "localhost";
        public DbSet<SwagGroup> SwagGroups { get; set; }
        public DbSet<SwagItem> SwagItems { get; set; }
        public DbSet<SwagSetting> SwagSettings { get; set; }
        public DbSet<SwagSettingGroup> SwagSettingGroups { get; set; }
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
            //SetSqlServerOptions(optionsBuilder);
            SetSqliteOptions(optionsBuilder);
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

    public class SwagGroupEntityConfiguration : IEntityTypeConfiguration<SwagGroup>
    {
        public void Configure(EntityTypeBuilder<SwagGroup> builder)
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
                .HasForeignKey<SwagItem>(si => si.GroupRootId)
                .OnDelete(DeleteBehavior.NoAction);

            //SwagGroupViewModel Many to One => SwagItemViewModel
            //builder.HasMany(sg => sg.Descendants)
            //    .WithOne(si => si.Group)
            //    .HasForeignKey(si => si.GroupId)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class SwagItemEntityConfiguration : IEntityTypeConfiguration<SwagItem>
    {
        public void Configure(EntityTypeBuilder<SwagItem> builder)
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
                    si => JsonHelper.ToJsonString(si),
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

    public class SwagSettingViewModelEntityConfiguration : IEntityTypeConfiguration<SwagSetting>
    {
        public void Configure(EntityTypeBuilder<SwagSetting> builder)
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

            Func<String, ConcurrentObservableOrderedDictionary<String, SwagDataColumn>> stringToDict = (str) =>
            {
                KeyValuePair<String, SwagDataColumn>[] cols = JsonConvert.DeserializeObject<KeyValuePair<String, SwagDataColumn>[]>(str, new SwagDataTableConverter());
                ConcurrentObservableOrderedDictionary<string, SwagDataColumn> dict = new ConcurrentObservableOrderedDictionary<string, SwagDataColumn>();
                foreach (KeyValuePair<String, SwagDataColumn> kvp in cols)
                {
                    dict.Add(kvp.Key, kvp.Value);
                }
                return dict;
            };

            //SwagItemViewModel Value
            builder.Property(sdt => sdt.Columns)
                .HasConversion(
                    sdc => JsonConvert.SerializeObject(sdc.List, Formatting.Indented),
                    sdc => stringToDict(sdc)
             );
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

    //https://www.jerriepelser.com/blog/custom-converters-in-json-net-case-study-1/
    public class SwagDataTableConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(KeyValuePair<String, SwagDataColumn>[]).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jArray = JArray.Load(reader);
            KeyValuePair<String, SwagDataColumn>[] pairs = new KeyValuePair<string, SwagDataColumn>[jArray.Count];

            for (int c = 0; c < jArray.Count; c++)
            {
                JToken jItem = jArray[c];
                String key = jItem["Key"].Value<String>();
                JObject jColumn = jItem["Value"].Value<JObject>();

                SwagDataColumn sdc = new SwagDataColumn();

                foreach (KeyValuePair<String, JToken> cProp in jColumn)
                {
                    if (cProp.Value is JValue)
                    {
                        JValue jVal = (JValue)cProp.Value;
                        ReflectionHelper.PropertyInfoCollection[typeof(SwagDataColumn)][cProp.Key].SetValue(sdc, jVal.Value);
                    }

                    if (cProp.Value is JObject && cProp.Key == "Binding")
                    {
                        JObject jBinding = (JObject)cProp.Value;
                        System.Windows.Data.Binding binding = new System.Windows.Data.Binding();

                        foreach (KeyValuePair<String, JToken> bProp in jBinding)
                        {
                            JValue jVal = (JValue)bProp.Value;
                            Object val = jVal.Value;

                            if (val != null)
                            {
                                switch (bProp.Key)
                                {
                                    case "Path":
                                        val = new System.Windows.PropertyPath(jVal.Value.ToString());
                                        break;
                                    case "Mode":
                                        val = (System.Windows.Data.BindingMode)Convert.ToInt32(jVal.Value);
                                        break;
                                    case "UpdateSourceTrigger":
                                        val = (System.Windows.Data.UpdateSourceTrigger)Convert.ToInt32(jVal.Value);
                                        break;
                                    case "Delay":
                                        val = Convert.ToInt32(jVal.Value);
                                        break;
                                }

                                ReflectionHelper.PropertyInfoCollection[typeof(System.Windows.Data.Binding)][bProp.Key].SetValue(binding, val);
                            }
                        }

                        sdc.Binding = binding;
                    }
                }

                pairs[c] = new KeyValuePair<string, SwagDataColumn>(key, sdc);
            }

            return pairs;
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
}
