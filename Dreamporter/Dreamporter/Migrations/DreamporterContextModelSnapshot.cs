﻿// <auto-generated />
using System;
using Dreamporter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dreamporter.Migrations
{
    [DbContext(typeof(DreamporterContext))]
    partial class DreamporterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Dreamporter.Core.BaseBuild", b =>
                {
                    b.Property<int>("BuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AlternateId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("CanUndo")
                        .HasColumnType("bit");

                    b.Property<string>("Condition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Display")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IntegrationId")
                        .HasColumnType("int");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsExpanded")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSelected")
                        .HasColumnType("bit");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("RequiredData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sequence")
                        .HasColumnType("int");

                    b.HasKey("BuildId");

                    b.HasIndex("ParentId");

                    b.ToTable("BaseBuilds");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseBuild");
                });

            modelBuilder.Entity("Dreamporter.Core.Integration", b =>
                {
                    b.Property<int>("IntegrationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BuildId")
                        .HasColumnType("int");

                    b.Property<string>("DataContexts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DefaultOptions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InitialSchemas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstructionTemplates")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OptionsSet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sequence")
                        .HasColumnType("int");

                    b.HasKey("IntegrationId");

                    b.HasIndex("BuildId")
                        .IsUnique();

                    b.ToTable("Integrations");
                });

            modelBuilder.Entity("Dreamporter.Core.Build", b =>
                {
                    b.HasBaseType("Dreamporter.Core.BaseBuild");

                    b.Property<string>("Instructions")
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("IntegrationId");

                    b.HasDiscriminator().HasValue("Build");
                });

            modelBuilder.Entity("Dreamporter.Core.GroupBuild", b =>
                {
                    b.HasBaseType("Dreamporter.Core.BaseBuild");

                    b.HasDiscriminator().HasValue("GroupBuild");
                });

            modelBuilder.Entity("Dreamporter.Core.BaseBuild", b =>
                {
                    b.HasOne("Dreamporter.Core.GroupBuild", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("Dreamporter.Core.Integration", b =>
                {
                    b.HasOne("Dreamporter.Core.GroupBuild", "Build")
                        .WithOne("Integration")
                        .HasForeignKey("Dreamporter.Core.Integration", "BuildId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Dreamporter.Core.Build", b =>
                {
                    b.HasOne("Dreamporter.Core.Integration", "Integration")
                        .WithMany()
                        .HasForeignKey("IntegrationId");
                });
#pragma warning restore 612, 618
        }
    }
}
