using Dreamporter.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System.Collections.Generic;

namespace Dreamporter.Data
{
    public class DreamporterContext : DbContext
    {
        #region Private Members
        string _dataSource = "localhost";
        #endregion Private Members

        #region Properties

        public DbSet<Integration> Integrations { get; set; }
        public DbSet<BaseBuild> BaseBuilds { get; set; }
        public DbSet<GroupBuild> GroupBuilds { get; set; }
        public DbSet<Build> Builds { get; set; }
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
            modelBuilder.ApplyConfiguration(new BaseBuildEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GroupBuildEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BuildEntityConfiguration());
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
                    .HasForeignKey<Integration>(i => i.BuildId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.Property(i => i.InstructionTemplates)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<List<Instruction>>(i));
                builder.Property(i => i.DefaultOptions)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<SwagOptionGroup>(i));
                builder.Property(i => i.OptionsSet)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<List<SwagOptionGroup>>(i));
                builder.Property(i => i.InitialSchemas)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<List<Schema>>(i));
                builder.Property(i => i.DataContexts)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<List<DataContext>>(i));
            }
        }
        #endregion IntegrationEntityConfiguration
        #region BaseBuildEntityConfiguration
        public class BaseBuildEntityConfiguration : IEntityTypeConfiguration<BaseBuild>
        {
            public void Configure(EntityTypeBuilder<BaseBuild> builder)
            {
                builder.HasKey(b => b.BuildId);
                builder.Property(b => b.Condition)
                    .HasConversion(
                        b => JsonHelper.ToJsonString(b),
                        b => JsonHelper.ToObject<BooleanContainerExpression>(b));
                builder.Property(b => b.RequiredData)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<List<Schema>>(i));
                
            }
        }
        #endregion BaseBuildEntityConfiguration
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
        #region BuildEntityConfiguration
        public class BuildEntityConfiguration : IEntityTypeConfiguration<Build>
        {
            public void Configure(EntityTypeBuilder<Build> builder)
            {
                
                builder.Property(b => b.Instructions)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<GroupInstruction>(i));
            }
        }
        #endregion CoreBuildEntityConfiguration
        #endregion Entity Configurations
    }
}
