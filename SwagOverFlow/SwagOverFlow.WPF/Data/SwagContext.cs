using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SwagOverFlow.ViewModels;
using SwagOverFlow.Utils;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace SwagOverFlow.WPF.Data
{
    public class SwagContext : DbContext
    {
        static string _dataSource = "localhost";
        public DbSet<SwagItemBase> SwagItems { get; set; }
        public DbSet<SwagSetting> SwagSettings { get; set; }
        public DbSet<SwagSettingGroup> SwagSettingGroups { get; set; }
        public DbSet<SwagSettingString> SwagSettingStrings { get; set; }
        public DbSet<SwagSettingBoolean> SwagSettingBooleans { get; set; }
        public DbSet<SwagData> SwagData { get; set; }
        public DbSet<SwagDataGroup> SwagDataGroups { get; set; }
        public DbSet<SwagDataTable> SwagDataTables { get; set; }
        public DbSet<SwagDataRow> SwagDataRows { get; set; }
        public DbSet<SwagDataColumn> SwagDataColumns { get; set; }
        public DbSet<SwagTabItem> SwagTabItems { get; set; }
        public SwagContext() : base() { }

        public SwagContext(DbContextOptions<SwagContext> options) : base (options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SwagItemBaseEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingGroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingStringEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagSettingBooleanEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataGroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataTableEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagDataRowEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SwagTabItemEntityConfiguration());
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
            //SwagIndexedItemViewModel Key + Id (AutoIncrement) => Key
            //Sqlite does not play too well with composite keys
            //builder.HasKey(si => new { si.GroupId, si.ItemId });
            builder.HasKey(si => si.ItemId);
            builder.Property(si => si.ItemId).ValueGeneratedOnAdd();

            //SwagIndexedItemViewModel AlternateId => Unique
            builder.HasIndex(si => si.AlternateId).IsUnique();

            //SwagIndexedItemViewModel Value
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
            //SwagSettingViewModel SettingType => Convert to String
            EnumToStringConverter<SettingType> settingTypeconverter = new EnumToStringConverter<SettingType>();
            builder.Property(ss => ss.SettingType).HasConversion(settingTypeconverter);

            //SwagSettingViewModel ObjItemsSource => Convert to String
            builder.Property(ss => ss.ObjItemsSource)
                .HasConversion(
                    ss => JsonHelper.ToJsonString(ss),
                    ss => JsonConvert.DeserializeObject<object>(ss));

            //SwagSetting Icon => Ignore
            builder.Ignore(ss => ss.Icon);
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

            //Func<String, SwagObservableOrderedDictionary<String, SwagDataColumn>> stringToSwagDict = (str) =>
            //{
            //    KeyValuePair<String, SwagDataColumn>[] cols = JsonConvert.DeserializeObject<KeyValuePair<String, SwagDataColumn>[]>(str, new SwagDataTableConverter());
            //    SwagObservableOrderedDictionary<string, SwagDataColumn> dict = new SwagObservableOrderedDictionary<string, SwagDataColumn>();
            //    foreach (KeyValuePair<String, SwagDataColumn> kvp in cols)
            //    {
            //        dict.Add(kvp.Key, kvp.Value);
            //    }
            //    return dict;
            //};

            //SwagDataTable Columns
            //builder.Property(sdt => sdt.Columns)
            //    .HasConversion(
            //        sdc => JsonConvert.SerializeObject(sdc, Formatting.Indented),
            //        sdc => stringToSwagDict(sdc)
            // );

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
            //        si => JsonHelper.ToObject<SwagTabCollection>(si));
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

    public class SwagTabItemEntityConfiguration : IEntityTypeConfiguration<SwagTabItem>
    {
        public void Configure(EntityTypeBuilder<SwagTabItem> builder)
        {
            //SwagSetting Icon => Ignore
            builder.Ignore(ss => ss.Icon);
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
            JObject jObject = JObject.Load(reader);
            KeyValuePair<String, SwagDataColumn>[] pairs = new KeyValuePair<string, SwagDataColumn>[jObject.Count];

            Int32 cIndex = 0;
            foreach (KeyValuePair<String, JToken> kvp in jObject)
            {
                String key = kvp.Key;
                JObject jColumn = (JObject)kvp.Value;

                SwagDataColumn sdc = new SwagDataColumn();

                foreach (KeyValuePair<String, JToken> cProp in jColumn)
                {
                    if (cProp.Value is JValue)
                    {
                        JValue jVal = (JValue)cProp.Value;
                        PropertyInfo propertyInfo = ReflectionHelper.PropertyInfoCollection[typeof(SwagDataColumn)][cProp.Key];
                        if (propertyInfo.SetMethod != null)
                        {
                            TypeConverter converter = ReflectionHelper.TypeConverterCache[propertyInfo.PropertyType];

                            if (jVal.Value != null && converter.CanConvertFrom(jVal.Value.GetType()))
                            {
                                propertyInfo.SetValue(sdc, converter.ConvertFrom(jVal.Value));
                            }
                            else if (jVal.Value != null && converter.CanConvertFrom(typeof(String))) //Try converting from string
                            {
                                propertyInfo.SetValue(sdc, converter.ConvertFrom(jVal.Value.ToString()));
                            }
                            else if (jVal.Value != null && propertyInfo.PropertyType == typeof(Type))
                            {
                                propertyInfo.SetValue(sdc, Type.GetType(jVal.Value.ToString()));
                            }
                            else
                            {
                                propertyInfo.SetValue(sdc, jVal.Value);
                            }
                        }
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

                        //TODO: FIX
                        //sdc.Binding = binding;
                    }
                }

                pairs[cIndex] = new KeyValuePair<string, SwagDataColumn>(key, sdc);
                cIndex++;
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
