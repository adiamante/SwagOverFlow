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
        public DbSet<CoreGroupBuild> CoreGroupBuilds { get; set; }
        public DbSet<ProfileGroupBuild> ProfileGroupBuilds { get; set; }
        public DbSet<CoreBuild> CoreBuilds { get; set; }
        public DbSet<ProfileBuild> ProfileBuilds { get; set; }
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
            modelBuilder.ApplyConfiguration(new CoreBuildEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileBuildEntityConfiguration());
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
                builder.HasOne(i => i.CoreBuild)
                    .WithOne(b => b.Integration)
                    .HasForeignKey<CoreGroupBuild>(b => b.IntegrationId);
                builder.HasOne(i => i.ProfileBuild)
                    .WithOne(b => b.Integration)
                    .HasForeignKey<ProfileGroupBuild>(b => b.IntegrationId);
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
                builder.Property(b => b.RequiredData)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<List<Schema>>(i));
                builder.Property(b => b.Instructions)
                    .HasConversion(
                        i => JsonHelper.ToJsonString(i),
                        i => JsonHelper.ToObject<GroupInstruction>(i));
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
        #region CoreBuildEntityConfiguration
        public class CoreBuildEntityConfiguration : IEntityTypeConfiguration<CoreBuild>
        {
            public void Configure(EntityTypeBuilder<CoreBuild> builder)
            {
                builder.Property(b => b.Condition)
                    .HasConversion(
                        b => JsonHelper.ToJsonString(b),
                        b => JsonHelper.ToObject<BooleanContainerExpression>(b));
            }
        }
        #endregion CoreBuildEntityConfiguration
        #region ProfileBuildEntityConfiguration
        public class ProfileBuildEntityConfiguration : IEntityTypeConfiguration<ProfileBuild>
        {
            public void Configure(EntityTypeBuilder<ProfileBuild> builder)
            {
                builder.Property(b => b.Options)
                    .HasConversion(
                        b => JsonHelper.ToJsonString(b),
                        b => JsonHelper.ToObject<SwagOptionGroup>(b));
            }
        }
        #endregion ProfileBuildEntityConfiguration
        #endregion Entity Configurations
    }
}
