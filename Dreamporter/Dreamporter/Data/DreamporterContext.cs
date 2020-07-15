using Dreamporter.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dreamporter.Data
{
    public class DreamporterContext : DbContext
    {
        #region Private Members
        string _dataSource = "localhost";
        #endregion Private Members

        #region Properties

        public DbSet<Integration> Integrations { get; set; }
        public DbSet<Build> Builds { get; set; }
        public DbSet<GroupBuild> GroupBuilds { get; set; }
        public DbSet<InstructionBuild> InstructionBuilds { get; set; }
        #endregion 

        #region Initialization
        public DreamporterContext() : base()
        {

        }

        public DreamporterContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IntegrationEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BuildEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GroupBuildEntityConfiguration());
            modelBuilder.ApplyConfiguration(new InstructionBuildEntityConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string migrationConnectionString = $"Data Source = { _dataSource }; Initial Catalog = Dreamporter2; Integrated Security = True";
            optionsBuilder.UseSqlServer(migrationConnectionString);
        }
        #endregion Initialization

        #region Entity Configurations
        #region IntegrationEntityConfiguration
        public class IntegrationEntityConfiguration : IEntityTypeConfiguration<Integration>
        {
            public void Configure(EntityTypeBuilder<Integration> builder)
            {
                builder.HasKey(i => i.IntegrationId);
                builder.HasOne(i => i.Build)
                    .WithOne(b => b.Integration)
                    .HasForeignKey<Integration>(i => i.BuildId);

                builder.Property(i => i.InstructionTemplates)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<GroupInstruction>(i));

                builder.Property(i => i.DefaultOptions)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<SwagOptionGroup>(i));

                Func<String, OptionsNode> stringToOptionsNod = (str) =>
                {
                    OptionsNode node = JsonHelper.ToObject<OptionsNode>(str);
                    node.Init();
                    return node;
                };
                builder.Property(i => i.OptionsTree)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => stringToOptionsNod(i));

                builder.Property(i => i.InitialSchemas)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<ObservableCollection<Schema>>(i));
                builder.Property(i => i.DataContexts)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<ObservableCollection<DataContext>>(i));
                builder.Property(i => i.SchemaGroups)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<ObservableCollection<String>>(i));
            }
        }
        #endregion IntegrationEntityConfiguration
        #region BuildEntityConfiguration
        public class BuildEntityConfiguration : IEntityTypeConfiguration<Build>
        {
            public void Configure(EntityTypeBuilder<Build> builder)
            {
                builder.HasKey(b => b.BuildId);
                builder.Property(b => b.Condition)
                    .HasConversion(
                        b => JsonHelper.ToJsonString(b),
                        b => JsonHelper.ToObject<BooleanContainerExpression>(b));
                builder.Property(b => b.RequiredData)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<ObservableCollection<Schema>>(i));
            }
        }
        #endregion BuildEntityConfiguration
        #region GroupBuildEntityConfiguration
        public class GroupBuildEntityConfiguration : IEntityTypeConfiguration<GroupBuild>
        {
            public void Configure(EntityTypeBuilder<GroupBuild> builder)
            {
                builder.HasMany(grp => grp.Children)
                    .WithOne(b => b.Parent)
                    .HasForeignKey(b => b.ParentId)
                    .OnDelete(DeleteBehavior.NoAction);
            }
        }
        #endregion GroupBuildEntityConfiguration
        #region InstructionBuildEntityConfiguration
        public class InstructionBuildEntityConfiguration : IEntityTypeConfiguration<InstructionBuild>
        {
            public void Configure(EntityTypeBuilder<InstructionBuild> builder)
            {
                builder.Property(b => b.Instructions)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<GroupInstruction>(i));
            }
        }
        #endregion InstructionBuildEntityConfiguration
        #endregion Entity Configurations
    }
}
